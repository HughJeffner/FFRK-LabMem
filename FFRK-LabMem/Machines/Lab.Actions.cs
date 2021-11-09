﻿using FFRK_LabMem.Data;
using FFRK_LabMem.Services;
using FFRK_Machines;
using FFRK_Machines.Machines;
using Newtonsoft.Json.Linq;
using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    public partial class Lab : Machine<Lab.State, Lab.Trigger, LabConfiguration>
    {

        private async Task DetermineState()
        {

            if (!Config.AutoStart || this.Data != null) return;
            try
            {
                await Config.Timings["Pre-AutoStart"].Wait(this.CancellationToken);
                ColorConsole.WriteLine(ConsoleColor.DarkGray, "Trying to auto-start");

                // Images to find
                List<Adb.ImageDef> items = new List<Adb.ImageDef>();
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_inventory, Simalarity = 0.90f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_skip, Simalarity = 0.90f });

                // Find
                var ret = await Adb.FindImages(items, 3, this.CancellationToken);
                if (ret != null)
                {
                    // Tap it
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Found area {0}", ret);

                    // Check inventory
                    if (ret.Equals(items[0]))
                    {
                        await Adb.TapPct(ret.Location.Item1, ret.Location.Item2, this.CancellationToken);
                        await Config.Timings["Inter-AutoStart"].Wait(this.CancellationToken);
                        await Adb.TapPct(5, 96, this.CancellationToken);
                    }

                    // Skip button
                    if (ret.Equals(items[1]))
                    {
                        await StateMachine.FireAsync(Trigger.BattleSuccess);
                    }

                    ColorConsole.WriteLine(ConsoleColor.DarkGray, "Auto-start complete, Have fun!");
                    await Config.Timings["Post-AutoStart"].Wait(this.CancellationToken);
                    return;
                }

            } catch (Exception)
            {

            }

            ColorConsole.WriteLine(ConsoleColor.DarkGray, "Could not auto-start");

        }

        private async Task SelectPainting()
        {
            // Check if safe disable requested
            if (await CheckDisableSafeRequested()) return;

            // Logic to determine painting
            CancellationToken.ThrowIfCancellationRequested();
            int total = (int)this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
            this.CurrentFloor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
            var paintings = (JArray)this.Data["labyrinth_dungeon_session"]["display_paintings"];
            this.CurrentPainting = null;

            // Is there a treasure vault or explore visible?
            var isTreasure = paintings.Any(p => (int)p["type"] == 3);
            var isExplore = paintings.Any(p => (int)p["type"] == 4);

            // Insert Priority Field in the first 3 items
            foreach (var item in paintings.Take(3))
            {
                item["priority"] = await GetPaintingPriority(item, isTreasure, isExplore, total, this.CurrentFloor.Equals(this.FinalFloor));
            }

            // Select top 1 priority from the first 3
            this.CurrentPainting = paintings
                .Take(3)                            // Only from the first 3
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])   // Priority ordering
                .ThenBy(p => rng.Next())            // Random for matching priority
                .FirstOrDefault();

            // Debug message
            if (Config.Debug)
            {
                StringBuilder builder = new StringBuilder("Priority: ");
                paintings.Take(3).ToList().ForEach(p => builder.AppendFormat("({0}) ", p["priority"]));
                ColorConsole.WriteLine(ConsoleColor.DarkGray, builder.ToString());
            }

            // Get selected painting id
            int selectedPaintingIndex = 0;
            if (this.CurrentPainting != null) selectedPaintingIndex = paintings.IndexOf(this.CurrentPainting);

            // Master painting check
            if ((int)this.CurrentPainting["type"] == 2 && (this.Config.StopOnMasterPainting || this.Config.UseTeleportStoneOnMasterPainting))
            {
                await this.StateMachine.FireAsync(Trigger.FoundBoss);
                return;
            }

            // Do Pick
            ColorConsole.Write("Picking painting {0}: {1}", selectedPaintingIndex + 1, this.CurrentPainting["name"]);
            if ((int)this.CurrentPainting["type"] <= 2)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", this.CurrentPainting["dungeon"]["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");
            await Config.Timings["Pre-SelectPainting"].Wait(this.CancellationToken);

            // TODO: clean this painting placement handling up
            // 2 or less paintings remaining change position
            if (total >= 3)
            {
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
                await Config.Timings["Inter-SelectPainting"].Wait(this.CancellationToken);
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
            }
            else if (total == 2)
            {
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
                await Config.Timings["Inter-SelectPainting"].Wait(this.CancellationToken);
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
            }
            else
            {
                await this.Adb.TapPct(50, 50, this.CancellationToken);
                await Config.Timings["Inter-SelectPainting"].Wait(this.CancellationToken);
                await this.Adb.TapPct(50, 50, this.CancellationToken);
            }

            CancellationToken.ThrowIfCancellationRequested();
            if ((int)this.CurrentPainting["type"] == 6)
            {
                await this.StateMachine.FireAsync(Trigger.PickedPortal);
            }

            await Config.Timings["Post-SelectPainting"].Wait(this.CancellationToken);

        }

        private async Task<int> GetPaintingPriority(JToken painting, bool isTreasure, bool isExplore, int total, bool isLastFloor)
        {

            // Type as string
            var type = painting["type"].ToString();

            // Radiant painting
            if ((bool)painting["is_special_effect"])
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Radiant painting detected!: {0}", painting["name"]);
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                if (Config.ScreenshotRadiantPainting)
                {
                    await Config.Timings["Pre-RadiantPaintingScreenshot"].Wait(this.CancellationToken);
                    await Adb.SaveScrenshot(String.Format("radiant_{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss")), this.CancellationToken);
                }
                return 0;
            }

            // Combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();

                // Enemy blocklist
                var enemyName = painting["dungeon"]["captures"][0]["tip_battle"]["title"].ToString();
                if (Config.EnemyBlocklist.Any(b => b.Enabled && enemyName.ToLower().Contains(b.Name.ToLower())))
                {
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Avoiding due to blocklist: {0}", enemyName);
                    return (Config.EnemyBlocklistAvoidOptionOverride ? 512 : 64);
                }
            }

            // There's a treasure or explore visible or more paintings not visible yet, but picked a portal
            if (type.Equals("6") && this.Config.AvoidPortal && (isTreasure || isExplore || (total > 9)))
            {
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Avoiding portal due to option");
                return 256;
            }

            // There's a treasure visible but explore (unless last floor)
            if (type.Equals("4") && this.Config.AvoidExploreIfTreasure && isTreasure && !isLastFloor)
            {
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Avoiding explore due to option");
                return 128;
            }

            // Lookup or default
            if (this.Config.PaintingPriorityMap.ContainsKey(type))
            {
                return this.Config.PaintingPriorityMap[type];
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Unknown painting id: {0}", type);
                return 1024;
            }

        }

        private async Task SelectTreasures()
        {

            // Got Item
            await DataLogger.LogGotItem(this);

            // Treasure list
            CancellationToken.ThrowIfCancellationRequested();
            var treasures = (JArray)this.Data["labyrinth_dungeon_session"]["treasure_chest_ids"];

            // Already picked this many
            int picked = treasures.Where(t => (int)t == 0).Count();

            // Treasure rate
            if (picked == 0) await DataLogger.LogTreasureRate(this, treasures);

            // Key usage
            int willSpendKeys = (picked * picked + picked) / 2;  // triangle number, n(n+1)/2

            // Select a random treasure
            JToken treasureToPick = treasures
                .Select(t => t)
                .Where(t => {
                    var filter = GetTreasureFilter(t);
                    return filter.Priority > 0 && filter.MaxKeys >= willSpendKeys;
                })
                .OrderBy(t => GetTreasureFilter(t).Priority)
                .ThenBy(t => GetTreasureFilter(t).MaxKeys)
                .ThenBy(t => rng.Next())
                .FirstOrDefault();

            // No treasures that match but we must pick one
            if (picked == 0 && treasureToPick == null)
                treasureToPick = treasures
                    .Select(t => t)
                    .OrderBy(t => rng.Next())
                    .FirstOrDefault();

            // Key check
            if (picked > this.CurrentKeys)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough keys!");
                treasureToPick = null;
            }

            // Pick if we got something good
            if (treasureToPick != null)
            {

                // Get item index
                int selectedTreasureIndex = 0;
                selectedTreasureIndex = treasures.IndexOf(treasureToPick);

                // Click chest
                ColorConsole.WriteLine("Picking treasure {0}", selectedTreasureIndex + 1);
                await Config.Timings["Pre-SelectTreasure"].Wait(this.CancellationToken);
                await this.Adb.TapPct(17 + (33 * (selectedTreasureIndex)), 50, this.CancellationToken);
                await Config.Timings["Inter-SelectTreasure"].Wait(this.CancellationToken);

                // Check if key needed
                if (picked > 0)
                {
                    ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Magic Key] x{0}", picked);
                    await this.Adb.TapPct(58, 44, this.CancellationToken);
                    await Config.Timings["Inter-SelectTreasure"].Wait(this.CancellationToken);
                }

                // Confirm
                await this.Adb.TapPct(70, 64, this.CancellationToken);

            }
            else
            {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                var b = await Adb.FindButtonAndTap("#2060ce", 4000, 40, 62, 80, 10, this.CancellationToken);
                if (b)
                {
                    await Config.Timings["Inter-SelectTreasure"].Wait(this.CancellationToken);
                    if (picked != 3)
                    {
                        await this.Adb.TapPct(70, 64, this.CancellationToken);
                        await Config.Timings["Inter-SelectTreasure"].Wait(this.CancellationToken);
                    }
                    await this.StateMachine.FireAsync(Trigger.MoveOn);
                }

            }

            await Config.Timings["Post-SelectTreasure"].Wait(this.CancellationToken);

        }

        private LabConfiguration.TreasureFilter GetTreasureFilter(JToken treasure)
        {

            var type = treasure.ToString().Substring(0, 1);

            if (this.Config.TreasureFilterMap.ContainsKey(type))
            {
                return this.Config.TreasureFilterMap[type];
            }
            else
            {
                if (!type.Equals("0")) ColorConsole.WriteLine(ConsoleColor.DarkRed, "Unknown treasure filter id: {0}", type);
                return new LabConfiguration.TreasureFilter() { MaxKeys = 0, Priority = 0 };
            }

        }

        private async Task OpenSealedDoor()
        {

            if (this.Config.OpenDoors)
            {
                ColorConsole.WriteLine("Opening Door...");
                await Config.Timings["Pre-Door"].Wait(this.CancellationToken);
                await this.Adb.TapPct(70, 74, this.CancellationToken);
            }
            else
            {
                ColorConsole.WriteLine("Leaving Door...");
                await Config.Timings["Pre-Door"].Wait(this.CancellationToken);
                await this.Adb.TapPct(30, 74, this.CancellationToken);
            }
            await Config.Timings["Post-Door"].Wait(this.CancellationToken);

        }

        private async Task MoveOn(bool FromPortal)
        {

            await DataLogger.LogGotItem(this);
            ColorConsole.WriteLine("Moving On...");
            await Config.Timings["Pre-MoveOn"].Wait(this.CancellationToken);

            var b = await Adb.FindButtonAndTap("#2060ce", 4000, 42.7, 65, 81, 30, this.CancellationToken);
            if (b)
            {
                await Config.Timings["Post-MoveOn"].Wait(this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.MoveOn);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

            // We need an additional delay if we got the dreaded portal
            if (FromPortal)
                await Config.Timings["Post-MoveOn-Portal"].Wait(this.CancellationToken);

        }

        private async Task EnterDungeon()
        {
            ColorConsole.WriteLine("Enter Dungeon");
            var b = await Adb.FindButtonAndTap("#2060ce", 2000, 56.6, 80, 95, 30, this.CancellationToken);
            if (b)
            {
                await this.StateMachine.FireAsync(Trigger.EnterDungeon);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

        }

        private async Task StartBattle()
        {
            ColorConsole.Write("Starting Battle");
            await Config.Timings["Pre-StartBattle"].Wait(this.CancellationToken);

            // Dungeon info
            var d = this.Data["labyrinth_dungeon_session"]["dungeon"];
            if (d != null)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", d["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");

            // Lethe Tears
            if (Config.UseLetheTears)
            {
                var gotFatigueValues = await fatigueAutoResetEvent.WaitAsync(TimeSpan.FromMilliseconds(Config.Timings["Pre-StartBattle-Fatigue"].DelayWithJitter), this.CancellationToken);
                if (gotFatigueValues)
                {
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Fatigue values READ: {0}", fatigueAutoResetEvent);
                    if (FatigueInfo.Any(f => (Config.LetheTearsSlot & (1 << 4 - FatigueInfo.IndexOf(f))) != 0 && f.Fatigue >= Config.LetheTearsFatigue))
                    {
                        await UseLetheTears();
                    }
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Timed out waiting for fatigue values");
                }
            }

            // Enter
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 42.7, 85, 95, 30, this.CancellationToken))
            {
                await Config.Timings["Inter-StartBattle"].Wait(this.CancellationToken);
                // Fatigue warning
                await Adb.FindButtonAndTap("#2060ce", 2000, 56, 55, 65, 5, this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.StartBattle);
                battleStopwatch.Start();
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }
            await Config.Timings["Post-StartBattle"].Wait(this.CancellationToken);

        }

        private async Task FinishBattle()
        {

            // Timer
            battleStopwatch.Stop();
            ColorConsole.Write("Battle Won!");
            ColorConsole.Write(ConsoleColor.DarkGray, " ({0:00}:{1:00})", battleStopwatch.Elapsed.Minutes, battleStopwatch.Elapsed.Seconds);
            battleStopwatch.Reset();

            // Drops
            await DataLogger.LogBattleDrops(this);

            // Update fatigue unknown value
            FatigueInfo.ForEach(f => f.Fatigue = -1);
            fatigueAutoResetEvent.Reset();

            // Check if safe disable requested
            if (await CheckDisableSafeRequested()) return;

            //Tappy taps
            await Config.Timings["Post-Battle"].Wait(this.CancellationToken);
            await this.Adb.TapPct(85, 85, this.CancellationToken);
            await Task.Delay(1000, this.CancellationToken);
            await this.Adb.TapPct(50, 85, this.CancellationToken);

            // Check if we defeated the boss
            if (this.Data != null && this.Data["result"]["labyrinth_dungeon_result"] != null)
                await this.StateMachine.FireAsync(Trigger.FinishedLab);


        }

        private async Task ConfirmPortal()
        {

            await Config.Timings["Pre-ConfirmPortal"].Wait(this.CancellationToken);
            await this.Adb.TapPct(71, 62, this.CancellationToken);
            await Config.Timings["Post-ConfirmPortal"].Wait(this.CancellationToken);

        }

        private async Task<bool> UseLetheTears()
        {

            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Lethe Tears] x{0}",
                Convert.ToString(Config.LetheTearsSlot, 2).ToCharArray().Count(c => c == '1'));

            await Config.Timings["Pre-LetheTears"].Wait(this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(88.88, 17.18, this.CancellationToken);
            await Config.Timings["Inter-LetheTears"].Wait(this.CancellationToken);

            // Each unit if selected
            for (int i = 0; i < 5; i++)
            {
                if ((Config.LetheTearsSlot & (1 << 4-i)) != 0)
                {
                    await this.Adb.TapPct(11.11 + (i * 15.55), 31.64, this.CancellationToken);
                    await Config.Timings["Inter-LetheTears-Unit"].Wait(this.CancellationToken);
                }
            }

            // Confirm button
            await Config.Timings["Inter-LetheTears"].Wait(this.CancellationToken);
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 37.5, 74, 87, 20, this.CancellationToken))
            {
                //Use Lethe Tears brown button
                await Config.Timings["Inter-LetheTears"].Wait(this.CancellationToken);
                if (await Adb.FindButtonAndTap("#6c3518", 2000, 50, 29, 42, 20, this.CancellationToken))
                {
                    // Confirmation
                    await Config.Timings["Inter-LetheTears"].Wait(this.CancellationToken);
                    if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                    {
                        // OK
                        await Config.Timings["Inter-LetheTears"].Wait(this.CancellationToken);
                        if (await Adb.FindButtonAndTap("#2060ce", 3000, 38.8, 55, 70, 5, this.CancellationToken))
                        {
                            await Config.Timings["Post-LetheTears"].Wait(this.CancellationToken);
                            return true;
                        } else
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'OK' button");
                        }
                        
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'Yes' button");
                    }
                    
                } else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'Use' button");
                }
                
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'Confirm' button");
            }
            return false;
        }

        private async Task<bool> UseTeleportStone()
        {

            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Teleport Stone] x1");
            await Config.Timings["Pre-TeleportStone"].Wait(this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(90.27, 4.68, this.CancellationToken);
            await Config.Timings["Inter-TeleportStone"].Wait(this.CancellationToken);

            // Use a stone brown button
            await Config.Timings["Inter-TeleportStone"].Wait(this.CancellationToken);
            if (await Adb.FindButtonAndTap("#6c3518", 2000, 58, 23, 37.5, 20, this.CancellationToken))
            {
                // Confirmation
                await Config.Timings["Inter-TeleportStone"].Wait(this.CancellationToken);
                if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                {
                    await Config.Timings["Post-TeleportStone"].Wait(this.CancellationToken);
                    return true;
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'OK' button");
                }

            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find 'Use' button");
            }
            return false;
        }

        private async Task FinishLab(StateMachine<State, Trigger>.Transition t)
        {

            // Disable machine
            if (t.Destination == State.WaitForBoss)
            {
                ColorConsole.Write(ConsoleColor.Green, "We reached the master painting.  ");
                if (Config.StopOnMasterPainting)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Press 'E' to enable when ready.");
                    await Notify();
                    base.OnMachineFinished();
                } 
                if (Config.UseTeleportStoneOnMasterPainting)
                {
                    ColorConsole.WriteLine("");
                    await UseTeleportStone();
                    await StateMachine.FireAsync(Trigger.FinishedLab);
                }
                
            }

            if (t.Destination == State.Completed)
            {
                
                // Notify complete
                await Notify();

                // Restart or not
                ColorConsole.Write(ConsoleColor.Green, "Lab run completed!");
                if (!Config.RestartLab)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, " Press 'E' to enable when ready.");
                    base.OnMachineFinished();
                }
                else
                {
                    ColorConsole.WriteLine("");
                    await this.StateMachine.FireAsync(Trigger.Restart);
                }
            }

        }

        private async Task RestartLabCountdown(TimeSpan duration, params double[] notifyAt)
        {
            var notifies = new List<double>(notifyAt);
            ColorConsole.WriteLine("Restarting Lab in {0} seconds...", duration.TotalSeconds);
            if (notifies.Contains(duration.TotalSeconds)) notifies.Remove(duration.TotalSeconds);
            var timer = new Stopwatch();
            timer.Start();
            while (timer.Elapsed <= duration)
            {
                await Task.Delay(500);
                int seconds = (int)Math.Floor(duration.TotalSeconds - timer.Elapsed.TotalSeconds);
                var notify = notifies.Where(n => n >= seconds).FirstOrDefault();
                if (notify > 0)
                {
                    ColorConsole.WriteLine("Restarting Lab in {0} seconds...", seconds);
                    notifies.Remove(notify);
                }
            }
        }

        public async Task RestartLab()
        {

            // Inital delay
            await RestartLabCountdown(TimeSpan.FromMilliseconds(Config.Timings["Pre-RestartLab"].DelayWithJitter), 60, 30, 10);
            ColorConsole.WriteLine("Restarting Lab");

            // Dungeon Complete
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Dismissing dungeon complete dialog");
            var closeButton = await Adb.FindButton("#6c3518", 2000, 39, 81, 91, 10, this.CancellationToken);
            if (closeButton != null)
            {
                await Adb.TapPct(closeButton.Item1, closeButton.Item2, this.CancellationToken);
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Dungeon complete dialog not present");
            }

            // Mission Complete
            await Config.Timings["Inter-RestartLab"].Wait(this.CancellationToken);
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for mission complete dialog");
            await Adb.FindButtonAndTap("#6c3518", 2000, 39, 61, 82, 5, this.CancellationToken);

            // Enter button 1
            await Config.Timings["Inter-RestartLab"].Wait(this.CancellationToken);
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 1");
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 50, 84, 94, 20, this.CancellationToken))
            {

                // Enter button 2
                await Config.Timings["Inter-RestartLab"].Wait(this.CancellationToken);
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 2");
                if (await Adb.FindButtonAndTap("#2060ce", 3000, 50, 80, 90, 20, this.CancellationToken))
                {

                    // Stamina dialog
                    await Config.Timings["Inter-RestartLab-Stamina"].Wait(this.CancellationToken);
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for stamina dialog");
                    var staminaButton = await Adb.FindButton("#6c3518", 2000, 50, 36, 50, 5, this.CancellationToken);
                    if (staminaButton != null)
                    {
                        if (Config.UsePotions)
                        {
                            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Stamina Potion] x1");
                            await Adb.TapPct(staminaButton.Item1, staminaButton.Item2, this.CancellationToken); // Select potions
                            await Config.Timings["Inter-RestartLab-Stamina"].Wait(this.CancellationToken);
                            await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken);  // Use potion
                            await Config.Timings["Inter-RestartLab-Stamina"].Wait(this.CancellationToken);
                            await Adb.FindButtonAndTap("#2060ce", 3000, 47, 57, 70, 5, this.CancellationToken);  // Potion used dialog
                            await Config.Timings["Inter-RestartLab-Stamina"].Wait(this.CancellationToken);
                            await Adb.FindButtonAndTap("#2060ce", 3000, 50, 80, 90, 20, this.CancellationToken); // Enter button 2 again
                            await Config.Timings["Inter-RestartLab-Stamina"].Wait(this.CancellationToken);

                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough stamina!");
                            OnMachineFinished();
                            return;
                        }

                    }

                    // Confirm equipment box or enter
                    await Config.Timings["Inter-RestartLab"].Wait(this.CancellationToken);
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 3");
                    if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                    {

                        // Enter if equipment confirmed, otherwise should find nothing
                        await Config.Timings["Inter-RestartLab"].Wait(this.CancellationToken);
                        if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for confirm equipment box");
                        if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                        {
                            await Config.Timings["Post-RestartLab"].Wait(this.CancellationToken);
                        }

                        // Reset state
                        await StateMachine.FireAsync(Trigger.ResetState);

                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 3");
                    }


                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 2");
                }

            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 1");
            }

        }

        private async Task RestartFFRK()
        {

            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting FFRK");

            // Kill FFRK
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Kill ffrk process...");
            await this.Adb.StopPackage(Adb.FFRK_PACKAGE_NAME, this.CancellationToken);
            await Config.Timings["Pre-RestartFFRK"].Wait(this.CancellationToken);

            // Launch app
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Launching app");
            await this.Adb.StartActivity(Adb.FFRK_PACKAGE_NAME, Adb.FFRK_ACTIVITY_NAME, this.CancellationToken);
            await Config.Timings["Pre-RestartFFRK"].Wait(this.CancellationToken);

            if (Config.UseOldCrashRecovery)
            {
                // Press start button
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Wating for start button...");
                if (await Adb.FindButtonAndTap("#2060ce", 4000, 40, 70, 83, 20, this.CancellationToken))
                {
                    // Press continue battle button
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Wating for continue battle button...");
                    if (await Adb.FindButtonAndTap("#2060ce", 4000, 61, 57, 68, 10, this.CancellationToken))
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery restarted battle");
                        await this.StateMachine.FireAsync(Trigger.StartBattle);
                    }
                    else
                    {
                        // Go back into lab
                        if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Tapping lab...");
                        await Adb.FindButtonAndTap("#d7b1fa", 4000, 50, 40, 60, 20, this.CancellationToken);
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery entered lab");
                        ConfigureStateMachine();
                    }

                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to detect FFRK restart");
                    OnMachineFinished();
                    await Notify();
                }

            }
            else
            {

                // Reset state
                ConfigureStateMachine();

                // Images to find
                List<Adb.ImageDef> items = new List<Adb.ImageDef>();
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_blue_play, Simalarity = 0.95f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_brown_ok, Simalarity = 0.95f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.lab_segment, Simalarity = 0.85f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.lab_outpost, Simalarity = 0.85f });

                // Stopwatch to limit how long we try to find buttons
                recoverStopwatch.Restart();

                // Button Finding Loop with timeout and break if stopwatch stopped
                double loopTimeout = TimeSpan.FromMilliseconds(Config.Timings["Inter-RestartFFRK-Timeout"].DelayWithJitter).TotalSeconds;
                bool labFinished = false;
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Button finding loop for {0}s", loopTimeout);
                while (recoverStopwatch.Elapsed < TimeSpan.FromSeconds(loopTimeout) && recoverStopwatch.IsRunning)
                {
                    // Find images in order, breaking on first match
                    var ret = await Adb.FindImages(items, 3, this.CancellationToken);
                    if (ret != null)
                    {
                        // Tap it
                        await Adb.TapPct(ret.Location.Item1, ret.Location.Item2, this.CancellationToken);

                        // Check for outpost
                        if (ret.Equals(items[3]))
                        {
                            labFinished = true;
                            break;
                        }

                    }
                    // Delay between finds
                    await Config.Timings["Inter-RestartFFRK"].Wait(this.CancellationToken);
                }

                // Loop finshed, check state
                CancellationToken.ThrowIfCancellationRequested();
                recoverStopwatch.Stop();

                // Disable lab if timeout
                if (recoverStopwatch.Elapsed > TimeSpan.FromSeconds(loopTimeout))
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "FFRK restart timed out");
                    OnMachineFinished();
                    await Notify();
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "FFRK restarted!");
                    if (labFinished) await StateMachine.FireAsync(Trigger.FinishedLab);
                }
                await Config.Timings["Post-RestartFFRK"].Wait(this.CancellationToken);
            }

        }

        private async Task RestartBattle()
        {

            await Config.Timings["Pre-RestartBattle"].Wait(this.CancellationToken);
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                await this.Adb.TapPct(50, 72, this.CancellationToken);
                await Config.Timings["Inter-RestartBattle"].Wait(this.CancellationToken);
                await this.Adb.TapPct(25, 55, this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.StartBattle);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Waiting for user input...");
                await Notify();
                watchdogTimer.Stop();
                if (this.Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Watchdog stopped");
            }
            await Config.Timings["Post-RestartBattle"].Wait(this.CancellationToken);

        }

    }
}
