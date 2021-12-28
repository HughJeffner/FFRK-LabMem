using FFRK_LabMem.Data;
using FFRK_LabMem.Services;
using FFRK_Machines;
using FFRK_Machines.Machines;
using FFRK_Machines.Services.Notifications;
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

        private const string BUTTON_BLUE = "#2060ce";
        private const string BUTTON_BROWN = "#6c3518";

        private readonly Dictionary<string, string> Combatant_Color = new Dictionary<string, string> { 
            { "1","G" },
            {"2", "O" },
            {"3", "R" }
        };

        private async Task DetermineState()
        {

            if (!Config.AutoStart || this.Data != null) return;
            try
            {
                await LabTimings.Delay("Pre-AutoStart", this.CancellationToken);
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
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Found area {0}", ret);

                    // Check inventory
                    if (ret.Equals(items[0]))
                    {
                        await Adb.TapPct(ret.Location.Item1, ret.Location.Item2, this.CancellationToken);
                        await LabTimings.Delay("Inter-AutoStart", this.CancellationToken);
                        await Adb.TapPct(5, 96, this.CancellationToken);
                    }

                    // Skip button
                    if (ret.Equals(items[1]))
                    {
                        await StateMachine.FireAsync(Trigger.BattleSuccess);
                    }

                    ColorConsole.WriteLine(ConsoleColor.DarkGray, "Auto-start complete, Have fun!");
                    await LabTimings.Delay("Post-AutoStart", this.CancellationToken);
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
                item["priority"] = await selector.GetPaintingPriority(item, isTreasure, isExplore, total, this.CurrentFloor.Equals(this.FinalFloor));
            }

            // Select top 1 priority from the first 3
            this.CurrentPainting = paintings
                .Take(3)                            // Only from the first 3
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])   // Priority ordering
                .ThenBy(p => rng.Next())            // Random for matching priority
                .FirstOrDefault();

            // Debug message
            if (ColorConsole.CheckCategory(ColorConsole.DebugCategory.Lab))
            {
                StringBuilder builder = new StringBuilder("Priority: ");
                paintings.Take(3).ToList().ForEach(p => builder.AppendFormat("({0}) ", p["priority"]));
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, builder.ToString());
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
                var title = this.CurrentPainting["dungeon"]["captures"][0]["tip_battle"]["title"].ToString();
                var color = this.CurrentPainting["display_type"];
                if (color != null && Combatant_Color.ContainsKey(color.ToString()))
                {
                    title += String.Format(" [{0}]", Combatant_Color[color.ToString()]);
                }
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", title);
                if (title.ToString().ToLower().Contains("magic pot")) await Counters.FoundMagicPot();
            }
            ColorConsole.WriteLine("");
            await LabTimings.Delay("Pre-SelectPainting", this.CancellationToken);

            // Tap painting
            // 2 or less paintings remaining change position
            int offset = (total>=2)?33:0;
            int margin = 17;
            if (total == 2) margin = 33;
            if (total == 1) margin = 50;

            await this.Adb.TapPct(margin + (offset * (selectedPaintingIndex)), 50, this.CancellationToken);
            await LabTimings.Delay("Inter-SelectPainting", this.CancellationToken);
            await this.Adb.TapPct(margin + (offset * (selectedPaintingIndex)), 50, this.CancellationToken);
           
            // Counter
            await Counters.PaintingSelected();

            // Clean up
            CancellationToken.ThrowIfCancellationRequested();
            if ((int)this.CurrentPainting["type"] == 6)
            {
                await this.StateMachine.FireAsync(Trigger.PickedPortal);
            }

            await LabTimings.Delay("Post-SelectPainting", this.CancellationToken);

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
                    var filter = selector.GetTreasureFilter(t);
                    return filter.Priority > 0 && filter.MaxKeys >= willSpendKeys;
                })
                .OrderBy(t => selector.GetTreasureFilter(t).Priority)
                .ThenBy(t => selector.GetTreasureFilter(t).MaxKeys)
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
                await LabTimings.Delay("Pre-SelectTreasure", this.CancellationToken);
                await this.Adb.TapPct(17 + (33 * (selectedTreasureIndex)), 50, this.CancellationToken);
                await LabTimings.Delay("Inter-SelectTreasure", this.CancellationToken);

                // Check if key needed
                if (picked > 0)
                {
                    ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Magic Key] x{0} of {1}", picked, CurrentKeys);
                    this.CurrentKeys -= picked;
                    await this.Adb.TapPct(58, 44, this.CancellationToken);
                    await LabTimings.Delay("Inter-SelectTreasure", this.CancellationToken);
                }

                // Confirm
                await this.Adb.TapPct(70, 64, this.CancellationToken);
                await Counters.UsedKeys(picked);
                await Counters.TreasureOpened();

            }
            else
            {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                var b = await Adb.FindButtonAndTap(BUTTON_BLUE, 4000, 40, 62, 80, 10, this.CancellationToken);
                if (b)
                {
                    await LabTimings.Delay("Inter-SelectTreasure", this.CancellationToken);
                    if (picked != 3)
                    {
                        await this.Adb.TapPct(70, 64, this.CancellationToken);
                        await LabTimings.Delay("Inter-SelectTreasure", this.CancellationToken);
                    }
                    await this.StateMachine.FireAsync(Trigger.MoveOn);
                }

            }

            await LabTimings.Delay("Post-SelectTreasure", this.CancellationToken);

        }

        private async Task OpenSealedDoor()
        {
            ColorConsole.WriteLine("{0} Door...", this.Config.OpenDoors ? "Opening" : "Leaving");
            await LabTimings.Delay("Pre-Door", this.CancellationToken);
            bool foundButton;
            if (this.Config.OpenDoors)
            {
                foundButton = await this.Adb.FindButtonAndTap(BUTTON_BLUE, 4000, 70, 66, 80, 3, this.CancellationToken);
            }
            else
            {
                foundButton = await this.Adb.FindButtonAndTap(BUTTON_BROWN, 4000, 30, 66, 80, 3, this.CancellationToken);
            }
            if (!foundButton) ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
            await LabTimings.Delay("Post-Door", this.CancellationToken);

        }

        private async Task MoveOn(bool FromPortal)
        {

            await DataLogger.LogGotItem(this);
            ColorConsole.WriteLine("Moving On...");
            await LabTimings.Delay("Pre-MoveOn", this.CancellationToken);

            var b = await Adb.FindButtonAndTap(BUTTON_BLUE, 4000, 42.7, 65, 81, 30, this.CancellationToken);
            if (b)
            {
                await LabTimings.Delay("Post-MoveOn", this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.MoveOn);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

            // We need an additional delay if we got the dreaded portal
            if (FromPortal)
                await LabTimings.Delay("Post-MoveOn-Portal", this.CancellationToken);

        }

        private async Task EnterDungeon()
        {
            ColorConsole.WriteLine("Enter Dungeon");
            var b = await Adb.FindButtonAndTap(BUTTON_BLUE, 2000, 56.6, 80, 95, 30, this.CancellationToken);
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
            await LabTimings.Delay("Pre-StartBattle", this.CancellationToken);

            // Dungeon info
            var d = this.Data["labyrinth_dungeon_session"]["dungeon"];
            if (d != null)
            {
                var title = d["captures"][0]["tip_battle"]["title"];
                ColorConsole.Write("The enemy is upon you! ");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "{0}", title);
                await Counters.EnemyIsUponYou();
                if (title.ToString().ToLower().Contains("magic pot")) await Counters.FoundMagicPot();
            } else
            {
                ColorConsole.WriteLine("Starting Battle");
            }

            // Do we need fatiuge values to proceed?
            if (
                (Config.UseLetheTears && (!Config.LetheTearsMasterOnly || (int)(this.CurrentPainting?["type"] ?? 0) == 2) // Using tears AND Not MasterOnly option or a master painting
                || Config.PartyIndex == LabConfiguration.PartyIndexOption.LowestFatigue)                                  // OR Lowest party fatigue option 
            )
            {
                // Wait for fatigue values downloaded on another thread
                var gotFatigueValues = await AutoResetEventFatigue.WaitAsync(await LabTimings.GetTimeSpan("Pre-StartBattle-Fatigue"), this.CancellationToken);
                if (gotFatigueValues)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Fatigue values READ: {0}", AutoResetEventFatigue);

                    // Select the party index using fatigue values
                    await LabTimings.Delay("Inter-StartBattle", this.CancellationToken);
                    await SelectParty(selector.GetPartyIndex());

                    // Fatigue level check for tears
                    if (SelectedPartyIndex < FatigueInfo.Count && 
                        FatigueInfo[SelectedPartyIndex].Any(f => 
                            (Config.LetheTearsSlot & (1 << 4 - FatigueInfo[SelectedPartyIndex].IndexOf(f))) != 0 && 
                            f.Fatigue >= Config.LetheTearsFatigue
                        )
                    ) await UseLetheTears();
   
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Timed out waiting for fatigue values");
                }
                
            } else
            {
                // Just select the party Index
                await LabTimings.Delay("Inter-StartBattle", this.CancellationToken);
                await SelectParty(selector.GetPartyIndex());
            }

            // Enter
            if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 42.7, 85, 95, 30, this.CancellationToken))
            {
                await LabTimings.Delay("Inter-StartBattle", this.CancellationToken);
                // Fatigue warning
                await Adb.FindButtonAndTap(BUTTON_BLUE, 2000, 56, 55, 65, 5, this.CancellationToken);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }
            await LabTimings.Delay("Post-StartBattle", this.CancellationToken);

        }

        private async Task SelectParty(int index)
        {
            SelectedPartyIndex = index;
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, $"Selecting party {index + 1}");
            
            // 0 already selected by default
            if (index == 0) return;
            if (index == 1)
            {
                await Adb.TapPct(50, 50, this.CancellationToken);
            } else
            {
                await Adb.TapPct(50, 66.7, this.CancellationToken);
            }

        }

        private async Task FinishBattle()
        {

            // Timer
            battleStopwatch.Stop();
            ColorConsole.Write("Battle Won!");
            ColorConsole.Write(ConsoleColor.DarkGray, " ({0:00}:{1:00})", battleStopwatch.Elapsed.Minutes, battleStopwatch.Elapsed.Seconds);
            await Counters.BattleWon(battleStopwatch.Elapsed);
            battleStopwatch.Reset();

            // Drops
            await DataLogger.LogBattleDrops(this);

            // Update fatigue unknown value
            if (SelectedPartyIndex < FatigueInfo.Count) FatigueInfo[SelectedPartyIndex].ForEach(f => f.Fatigue = -1);
            AutoResetEventFatigue.Reset();

            // Check if safe disable requested
            if (await CheckDisableSafeRequested()) return;

            //Tappy taps
            await LabTimings.Delay("Post-Battle", this.CancellationToken);
            await this.Adb.TapPct(85, 85, this.CancellationToken);
            await Task.Delay(1000, this.CancellationToken);
            await this.Adb.TapPct(50, 85, this.CancellationToken);

            // Check if we defeated the boss
            if (this.Data != null && this.Data["result"] != null && this.Data["result"]["labyrinth_dungeon_result"] != null)
                await this.StateMachine.FireAsync(Trigger.FinishedLab);


        }

        private async Task ConfirmPortal()
        {

            await LabTimings.Delay("Pre-ConfirmPortal", this.CancellationToken);
            await this.Adb.TapPct(71, 62, this.CancellationToken);
            await LabTimings.Delay("Post-ConfirmPortal", this.CancellationToken);

        }

        private async Task<bool> UseLetheTears()
        {

            int numberUsed = Convert.ToString(Config.LetheTearsSlot, 2).ToCharArray().Count(c => c == '1');

            // Check remaining qty
            if (numberUsed > CurrentTears){
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough lethe tears!");
                return true;
            }

            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Lethe Tears] x{0} of {1}", numberUsed, CurrentTears);
            await LabTimings.Delay("Pre-LetheTears", this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(88.88, 17.18, this.CancellationToken);
            await LabTimings.Delay("Inter-LetheTears", this.CancellationToken);

            // Each unit if selected
            for (int i = 0; i < 5; i++)
            {
                if ((Config.LetheTearsSlot & (1 << 4-i)) != 0)
                {
                    await this.Adb.TapPct(11.11 + (i * 15.55), 31.64, this.CancellationToken);
                    await LabTimings.Delay("Inter-LetheTears-Unit", this.CancellationToken);
                }
            }

            // Confirm button
            await LabTimings.Delay("Inter-LetheTears", this.CancellationToken);
            if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 37.5, 74, 87, 20, this.CancellationToken))
            {
                //Use Lethe Tears brown button
                await LabTimings.Delay("Inter-LetheTears", this.CancellationToken);
                if (await Adb.FindButtonAndTap(BUTTON_BROWN, 2000, 50, 29, 42, 20, this.CancellationToken))
                {
                    // Confirmation
                    await LabTimings.Delay("Inter-LetheTears", this.CancellationToken);
                    if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 57, 70, 5, this.CancellationToken))
                    {
                        // OK
                        await LabTimings.Delay("Inter-LetheTears", this.CancellationToken);
                        if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 38.8, 55, 70, 5, this.CancellationToken))
                        {
                            await LabTimings.Delay("Post-LetheTears", this.CancellationToken);
                            this.CurrentTears -= numberUsed;
                            await Counters.UsedTears(numberUsed);
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
            await LabTimings.Delay("Pre-TeleportStone", this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(90.27, 4.68, this.CancellationToken);
            await LabTimings.Delay("Inter-TeleportStone", this.CancellationToken);

            // Use a stone brown button
            await LabTimings.Delay("Inter-TeleportStone", this.CancellationToken);
            if (await Adb.FindButtonAndTap(BUTTON_BROWN, 2000, 58, 23, 37.5, 20, this.CancellationToken))
            {
                // Confirmation
                await LabTimings.Delay("Inter-TeleportStone", this.CancellationToken);
                if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 57, 70, 5, this.CancellationToken))
                {
                    await LabTimings.Delay("Post-TeleportStone", this.CancellationToken);
                    await Counters.UsedTeleportStone();
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
                    await Notify(Notifications.EventType.LAB_COMPLETED);
                    base.OnMachineFinished();
                    await Counters.LabRunCompleted();
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
                
                // Notify complete (only if not restarting)
                if (t.Source != State.Unknown)
                {
                    await Notify(Notifications.EventType.LAB_COMPLETED);
                    await Counters.LabRunCompleted();
                }

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
                await Task.Delay(500, this.CancellationToken);
                int seconds = (int)Math.Floor(duration.TotalSeconds - timer.Elapsed.TotalSeconds);
                var notify = notifies.Where(n => n == seconds).FirstOrDefault();
                if (notify > 0)
                {
                    ColorConsole.WriteLine("Restarting Lab in {0} seconds...", seconds);
                    notifies.Remove(notify);
                }
            }
        }

        private async Task RestartLab()
        {

            // Inital delay
            Watchdog.Kick(false);
            await RestartLabCountdown(await LabTimings.GetTimeSpan("Pre-RestartLab"), 60, 30, 10);
            this.CancellationToken.ThrowIfCancellationRequested();
            Watchdog.Kick(true);
            ColorConsole.WriteLine("Restarting Lab");

            // Dungeon Complete
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Dismissing dungeon complete dialog");
            var closeButton = await Adb.FindButton(BUTTON_BROWN, 2000, 39, 81, 91, 10, this.CancellationToken);
            if (closeButton != null)
            {
                await Adb.TapPct(closeButton.Item1, closeButton.Item2, this.CancellationToken);
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Dungeon complete dialog not present");
            }

            // Mission Complete
            await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for mission complete dialog");
            await Adb.FindButtonAndTap(BUTTON_BROWN, 2000, 39, 61, 82, 5, this.CancellationToken);
            
            // Enter button 1
            Watchdog.Kick(true);
            await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 1");
            if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 50, 84, 94, 20, this.CancellationToken))
            {

                // Enter button 2
                Watchdog.Kick(true);
                await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 2");
                if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 50, 80, 90, 20, this.CancellationToken))
                {

                    // Stamina dialog
                    Watchdog.Kick(true);
                    await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
                    var staminaResult = await CheckRestoreStamina();
                    if (staminaResult.PotionUsed)
                    {
                        // Enter button 2 again
                        await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 50, 80, 90, 20, this.CancellationToken); 
                        await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
                    }
                    else
                    {
                        if (staminaResult.StaminaDialogPresent) return;
                    }
   
                    // Confirm equipment box or enter
                    Watchdog.Kick(true);
                    await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 3");
                    if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 57, 70, 5, this.CancellationToken))
                    {

                        // Enter if equipment confirmed, otherwise should find nothing
                        await LabTimings.Delay("Inter-RestartLab", this.CancellationToken);
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for confirm equipment box");
                        if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 57, 70, 5, this.CancellationToken))
                        {
                            await LabTimings.Delay("Post-RestartLab", this.CancellationToken);
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
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for inventory full");
                    var b = await Adb.FindButton(BUTTON_BROWN, 3000, 40.2, 88.3, 97.7, 3, this.CancellationToken);
                    if (b!= null)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Inventory full!");
                        await Notify(Notifications.EventType.LAB_FAULT);
                        OnMachineFinished();
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 2");
                    }
                    
                }

            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 1");
            }

        }

        private class CheckRestoreStaminaResult
        {
            public bool StaminaDialogPresent { get; set; } = false;
            public bool PotionUsed { get; set; } = false;
        }

        private async Task<CheckRestoreStaminaResult> CheckRestoreStamina()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for restore stamina dialog");
            var ret = new CheckRestoreStaminaResult();

            var staminaButton = await Adb.FindButton(BUTTON_BROWN, 2000, 50, 36, 50, 5, this.CancellationToken);
            if (staminaButton != null)
            {
                ret.StaminaDialogPresent = true;
                if (Config.UsePotions)
                {
                    // Select potions
                    await Adb.TapPct(staminaButton.Item1, staminaButton.Item2, this.CancellationToken);

                    // Use potion
                    ret.PotionUsed = await UseStaminaPotion();
                    return ret;
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough stamina!");
                    OnMachineFinished();
                }

            }
            return ret;
        }

        private async Task<bool> UseStaminaPotion()
        {

            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Stamina Potion] x1");
            await LabTimings.Delay("Pre-StaminaPotion", this.CancellationToken);

            // Use potion
            if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 60, 80, 5, this.CancellationToken))
            {
                await LabTimings.Delay("Inter-StaminaPotion", this.CancellationToken);
                
                // Confirm qty
                if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 61, 55, 75, 5, this.CancellationToken)){

                    await Counters.UsedStaminaPot();
                    await LabTimings.Delay("Inter-StaminaPotion", this.CancellationToken);

                    // Potion used dialog
                    if (await Adb.FindButtonAndTap(BUTTON_BLUE, 3000, 47, 55, 70, 5, this.CancellationToken))
                    {
                        await LabTimings.Delay("Post-StaminaPotion", this.CancellationToken);
                        return true;
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find potion used dialog");
                    }

                } else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find potion confirm qty dialog");
                }

            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find use potion dialog");
            }

            return false;
        }

        private async Task<bool> RestartFFRK()
        {

            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting FFRK");

            // Pause watchdog
            Watchdog.Kick(false);

            // Stop any running tasks
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Cancelling any running tasks");
            await InterruptTasks();
            await LabTimings.Delay("Pre-RestartFFRK", this.CancellationToken);

            // Kill FFRK
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Kill ffrk process...");
            await this.Adb.StopPackage(Adb.FFRK_PACKAGE_NAME, this.CancellationToken);
            await LabTimings.Delay("Pre-RestartFFRK", this.CancellationToken);

            // Launch app
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Launching app");
            await this.Adb.StartActivity(Adb.FFRK_PACKAGE_NAME, Adb.FFRK_ACTIVITY_NAME, this.CancellationToken);

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
            TimeSpan loopTimeout = await LabTimings.GetTimeSpan("Inter-RestartFFRK-Timeout");
            bool labFinished = false;
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Button finding loop for {0}s", loopTimeout.TotalSeconds);
            while (recoverStopwatch.Elapsed < loopTimeout && recoverStopwatch.IsRunning)
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
                await LabTimings.Delay("Inter-RestartFFRK", this.CancellationToken);
            }

            // Loop finshed, check state
            CancellationToken.ThrowIfCancellationRequested();
            recoverStopwatch.Stop();

            // Disable lab if timeout
            if (recoverStopwatch.Elapsed > loopTimeout)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "FFRK restart timed out");
                // Interrupt tasks if restart failed just in case 
                await InterruptTasks();
                return false;
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "FFRK restarted!");
                await Counters.FFRKRestarted();
                if (labFinished) await StateMachine.FireAsync(Trigger.FinishedLab);
            }
            await LabTimings.Delay("Post-RestartFFRK", this.CancellationToken);
            return true;

        }

        private async Task RestartBattle()
        {

            await LabTimings.Delay("Pre-RestartBattle", this.CancellationToken);
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                await this.Adb.TapPct(50, 72, this.CancellationToken);
                await LabTimings.Delay("Inter-RestartBattle", this.CancellationToken);
                await this.Adb.TapPct(25, 55, this.CancellationToken);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Waiting for user input...");
                await Notify(Notifications.EventType.LAB_FAULT);
                Watchdog.Kick(false);
            }
            await LabTimings.Delay("Post-RestartBattle", this.CancellationToken);

        }

        public async Task<bool> QuickExplore()
        {

            // Inital delay
            Watchdog.Kick(false); // Pause the watchdog
            await LabTimings.Delay("Pre-QuickExplore", this.CancellationToken);
           
            // Dungeon Complete
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Dismissing complete dialog");
            var closeButton = await Adb.FindButton(BUTTON_BROWN, 2000, 39, 81, 91, 3, this.CancellationToken);
            if (closeButton != null)
            {
                await Adb.TapPct(closeButton.Item1, closeButton.Item2, this.CancellationToken);
            }
            else
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Complete dialog not present");
            }

            // Quick Explore Button
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing quick explore");
            await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
            var quickExploreButton = await Adb.FindButton(BUTTON_BROWN, 2000, 82.6, 83.2, 93.7, 10, this.CancellationToken);
            if (quickExploreButton != null)
            {
                // Tap It
                await Adb.TapPct(quickExploreButton.Item1, quickExploreButton.Item2, CancellationToken);

                // Use a record marker button
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing use a record marker");
                await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                var useRecordMarkerButton = await Adb.FindButton(BUTTON_BROWN, 2000, 50, 25.7, 36.3, 2, this.CancellationToken);
                
                // If record marker button not present then stamina too low
                if (useRecordMarkerButton == null)
                {

                    // Stamina Check
                    await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                    var staminaResult = await CheckRestoreStamina();
                    if (staminaResult.PotionUsed)
                    {
                        // Quick Explore Button Again
                        await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                        await Adb.TapPct(quickExploreButton.Item1, quickExploreButton.Item2, CancellationToken);

                        // Look for the record marker button again
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing use a record marker");
                        await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                        useRecordMarkerButton = await Adb.FindButton(BUTTON_BROWN, 2000, 50, 25.7, 39, 2, this.CancellationToken);
                    }
                    else
                    {
                        // Not enough stamina or just couldn't find the use record marker brown button
                        if (!staminaResult.StaminaDialogPresent) ColorConsole.WriteLine(ConsoleColor.DarkRed, "Use a record marker button not present");
                        return false;
                    }

                }
                               
                // Check for use record marker button again, needed if stamina restored
                if (useRecordMarkerButton != null)
                {

                    // Tap It
                    await Adb.TapPct(useRecordMarkerButton.Item1, useRecordMarkerButton.Item2, CancellationToken);

                    // Record marker will be used - OK
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing use record marker OK button");
                    await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                    if (await Adb.FindButtonAndTap(BUTTON_BLUE, 2000, 64, 54.6, 70.3, 5, this.CancellationToken))
                    {

                        // Stamina Cost
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing stamina cost OK button");
                        await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                        if (await Adb.FindButtonAndTap(BUTTON_BLUE, 2000, 64, 54, 70.3, 5, this.CancellationToken))
                        {

                            // Wait for results
                            if (await AutoResetEventQuickExplore.WaitAsync(await LabTimings.GetTimeSpan("Inter-QuickExplore-Timeout"), this.CancellationToken)){
                                await LabTimings.Delay("Post-QuickExplore", this.CancellationToken);
                                return true;
                            } else
                            {
                                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Timed out waiting for QE results");
                            }

                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Stamina cost dialog not present");
                        }

                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "OK button not present");
                    }

                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Use a record marker button not present");
                }
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Quick explore button not present");
            }

            return false;

        }

    }
}
