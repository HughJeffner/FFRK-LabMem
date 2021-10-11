using FFRK_LabMem.Data;
using FFRK_LabMem.Services;
using FFRK_Machines;
using FFRK_Machines.Machines;
using Newtonsoft.Json.Linq;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    public partial class Lab : Machine<Lab.State, Lab.Trigger, LabConfiguration>
    {

        private void DetermineState()
        {

        }

        private async Task SelectPainting()
        {

            // Logic to determine painting
            CancellationToken.ThrowIfCancellationRequested();
            int total = (int)this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
            this.CurrentFloor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
            var paintings = (JArray)this.Data["labyrinth_dungeon_session"]["display_paintings"];
            this.CurrentPainting = null;

            // New floor marker
            if (total == 20) ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", this.CurrentFloor);

            // Insert Priority Field
            foreach (var item in paintings)
            {
                item["priority"] = GetPaintingPriority(item);
            }

            // Is there a treasure vault or explore visible?
            var isTreasure = paintings.Any(p => (int)p["type"] == 3);
            var isExplore = paintings.Any(p => (int)p["type"] == 4);

            // Select top 1 priority from the first 3
            this.CurrentPainting = paintings
                .Take(3)                            // Only from the first 3
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])   // Priority ordering
                .ThenBy(p => rng.Next())            // Random for matching priority
                .FirstOrDefault();

            // There's a treasure visible but picked a explore (unless last floor)
            // TODO: Determine if the last floor
            if (this.Config.AvoidExploreIfTreasure && isTreasure && (int)this.CurrentPainting["type"] == 4 && this.CurrentFloor != 15 && this.CurrentFloor != 20)
            {
                this.CurrentPainting = paintings
                .Take(3)
                .Select(p => p)
                .Where(p => (int)p["type"] != 4)
                .OrderBy(p => (int)p["priority"])
                .FirstOrDefault();

                // No choice
                if (this.CurrentPainting == null) this.CurrentPainting = paintings[rng.Next(2)];

            }

            // There's a treasure or explore visible or more paintings not visible yet, but picked a portal
            if (this.Config.AvoidPortal && (int)this.CurrentPainting["type"] == 6 && (isTreasure || isExplore || (total > 9)))
            {
                this.CurrentPainting = paintings
                .Take(3)
                .Select(p => p)
                .Where(p => (int)p["type"] != 6)
                .OrderBy(p => (int)p["priority"])
                .FirstOrDefault();
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
            await Task.Delay(Config.Timings["Pre-SelectPainting"], this.CancellationToken);

            // TODO: clean this painting placement handling up
            // 2 or less paintings remaining change position
            if (total >= 3)
            {
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
                await Task.Delay(Config.Timings["Inter-SelectPainting"], this.CancellationToken);
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
            }
            else if (total == 2)
            {
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
                await Task.Delay(Config.Timings["Inter-SelectPainting"], this.CancellationToken);
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50, this.CancellationToken);
            }
            else
            {
                await this.Adb.TapPct(50, 50, this.CancellationToken);
                await Task.Delay(Config.Timings["Inter-SelectPainting"], this.CancellationToken);
                await this.Adb.TapPct(50, 50, this.CancellationToken);
            }


            // Change state if needed
            //if (new List<int>() { 7, 5 }.Contains((int)selectedPainting["type"]))
            //{
            //    await this.StateMachine.FireAsync(Trigger.FoundThing);
            //}

            if ((int)this.CurrentPainting["type"] == 6)
            {
                await this.StateMachine.FireAsync(Trigger.PickedPortal);
            }

        }

        private int GetPaintingPriority(JToken painting)
        {

            var type = painting["type"].ToString();

            if ((bool)painting["is_special_effect"])
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Radiant painting detected!: {0}", painting["name"]);
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                return 0;
            }

            // Subtype for combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();
            }

            if (this.Config.PaintingPriorityMap.ContainsKey(type))
            {
                return this.Config.PaintingPriorityMap[type];
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Unknown painting id: {0}", type);
                return 99;
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
            if (willSpendKeys > this.CurrentKeys)
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
                await Task.Delay(Config.Timings["Pre-SelectTreasure"], this.CancellationToken);
                await this.Adb.TapPct(17 + (33 * (selectedTreasureIndex)), 50, this.CancellationToken);
                await Task.Delay(2000, this.CancellationToken);

                // Check if key needed
                if (picked > 0)
                {
                    await this.Adb.TapPct(58, 44, this.CancellationToken);
                    await Task.Delay(2000, this.CancellationToken);
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
                    await Task.Delay(2000, this.CancellationToken);
                    if (picked != 3)
                    {
                        await this.Adb.TapPct(70, 64, this.CancellationToken);
                        await Task.Delay(2000, this.CancellationToken);
                    }
                    await this.StateMachine.FireAsync(Trigger.MoveOn);
                }

            }

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
                if (!type.Equals("0")) ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Unknown treasure filter id: {0}", type);
                return new LabConfiguration.TreasureFilter() { MaxKeys = 0, Priority = 0 };
            }

        }

        private async Task OpenSealedDoor()
        {

            if (this.Config.OpenDoors)
            {
                ColorConsole.WriteLine("Opening Door...");
                await Task.Delay(Config.Timings["Pre-Door"], this.CancellationToken);
                await this.Adb.TapPct(70, 74, this.CancellationToken);
                await Task.Delay(1000, this.CancellationToken);
            }
            else
            {
                ColorConsole.WriteLine("Leaving Door...");
                await Task.Delay(Config.Timings["Pre-Door"], this.CancellationToken);
                await this.Adb.TapPct(30, 74, this.CancellationToken);
                await Task.Delay(1000, this.CancellationToken);
            }

        }

        private async Task MoveOn()
        {

            await DataLogger.LogGotItem(this);
            ColorConsole.WriteLine("Moving On...");
            await Task.Delay(Config.Timings["Pre-MoveOn"], this.CancellationToken);

            var b = await Adb.FindButtonAndTap("#2060ce", 4000, 42.7, 65, 81, 30, this.CancellationToken);
            if (b)
            {
                await Task.Delay(1000, this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.MoveOn);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

            // Failed

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
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

        }

        private async Task StartBattle()
        {
            ColorConsole.Write("Starting Battle");
            this.CancellationToken.ThrowIfCancellationRequested();

            // Dungeon info
            var d = this.Data["labyrinth_dungeon_session"]["dungeon"];
            if (d != null)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", d["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");

            // Lethe Tears
            var gotFatigueValues = await fatigueAutoResetEvent.WaitAsync(TimeSpan.FromSeconds(20), this.CancellationToken);
            if (gotFatigueValues)
            {
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Fatigue values READ: {0}", fatigueAutoResetEvent);
                if (Config.UseLetheTears && FatigueInfo.Any(f => (Config.LetheTearsSlot & (1 << 4 - FatigueInfo.IndexOf(f))) != 0 && f.Fatigue >= Config.LetheTearsFatigue))
                {
                    await UseLetheTears();
                }

            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Timed out waiting for fatigue values");
            }

            // Enter
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 42.7, 85, 95, 30, this.CancellationToken))
            {
                await Task.Delay(500, this.CancellationToken);
                // Fatigue warning
                await Adb.FindButtonAndTap("#2060ce", 2000, 56, 55, 65, 5, this.CancellationToken);
                await this.StateMachine.FireAsync(Trigger.StartBattle);
                battleStopwatch.Start();
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

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

            //Tappy taps
            await Task.Delay(Config.Timings["Post-Battle"], this.CancellationToken);
            await this.Adb.TapPct(85, 85, this.CancellationToken);
            await Task.Delay(1000, this.CancellationToken);
            await this.Adb.TapPct(50, 85, this.CancellationToken);

            // Check if we defeated the boss
            if (this.Data["result"]["labyrinth_dungeon_result"] != null)
                await this.StateMachine.FireAsync(Trigger.FinishedLab);


        }

        private async Task ConfirmPortal()
        {

            await Task.Delay(5000, this.CancellationToken);
            await this.Adb.TapPct(71, 62, this.CancellationToken);
            await Task.Delay(2000, this.CancellationToken);

        }

        private async Task<bool> UseLetheTears()
        {

            ColorConsole.WriteLine("Using Lethe Tears");
            await Task.Delay(4000, this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(88.88, 17.18, this.CancellationToken);
            await Task.Delay(2000, this.CancellationToken);

            // Each unit if selected
            for (int i = 0; i < 5; i++)
            {
                if ((Config.LetheTearsSlot & (1 << 4-i)) != 0)
                {
                    await this.Adb.TapPct(11.11 + (i * 15.55), 31.64, this.CancellationToken);
                    await Task.Delay(500, this.CancellationToken);
                }
            }

            // Confirm button
            await Task.Delay(2000, this.CancellationToken);
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 37.5, 74, 87, 20, this.CancellationToken))
            {
                //Use Lethe Tears brown button
                await Task.Delay(2000, this.CancellationToken);
                if (await Adb.FindButtonAndTap("#6c3518", 2000, 50, 29, 42, 20, this.CancellationToken))
                {
                    // Confirmation
                    await Task.Delay(2000, this.CancellationToken);
                    if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                    {
                        // OK
                        await Task.Delay(2000, this.CancellationToken);
                        if (await Adb.FindButtonAndTap("#2060ce", 3000, 38.8, 55, 70, 5, this.CancellationToken))
                        {
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

            ColorConsole.WriteLine("Using Teleport Stone");
            await Task.Delay(2000, this.CancellationToken);

            // Lethe tears button
            await this.Adb.TapPct(90.27, 4.68, this.CancellationToken);
            await Task.Delay(2000, this.CancellationToken);

            // Use a stone brown button
            await Task.Delay(2000, this.CancellationToken);
            if (await Adb.FindButtonAndTap("#6c3518", 2000, 58, 23, 37.5, 20, this.CancellationToken))
            {
                // Confirmation
                await Task.Delay(2000, this.CancellationToken);
                if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                {
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

        private async Task RestartLab()
        {

            // Inital delay
            ColorConsole.WriteLine("Restarting Lab in 60 seconds...");
            await Task.Delay(30000, this.CancellationToken);
            ColorConsole.WriteLine("Restarting Lab in 30 seconds...");
            await Task.Delay(20000, this.CancellationToken);
            ColorConsole.WriteLine("Restarting Lab in 10 seconds...");
            await Task.Delay(10000, this.CancellationToken);
            ColorConsole.WriteLine("Restarting Lab");

            // Dungeon Complete
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Dismissing dungeon complete dialog");
            if (await Adb.FindButtonAndTap("#6c3518", 2000, 39, 81, 91, 20, this.CancellationToken))
            {

                // Mission Complete
                await Task.Delay(5000, this.CancellationToken);
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for mission complete dialog");
                await Adb.FindButtonAndTap("#6c3518", 2000, 39, 61, 82, 5, this.CancellationToken);

                // Enter button 1
                await Task.Delay(5000, this.CancellationToken);
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 1");
                if (await Adb.FindButtonAndTap("#2060ce", 3000, 50, 84, 94, 20, this.CancellationToken))
                {

                    // Enter button 2
                    await Task.Delay(5000, this.CancellationToken);
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 2");
                    if (await Adb.FindButtonAndTap("#2060ce", 3000, 50, 80, 90, 20, this.CancellationToken))
                    {

                        // Stamina dialog
                        await Task.Delay(2000, this.CancellationToken);
                        if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for stamina dialog");
                        var button = await Adb.FindButton("#6c3518", 2000, 50, 36, 50, 5, this.CancellationToken);
                        if (button != null)
                        {
                            if (Config.UsePotions)
                            {
                                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Using a potion");
                                await Adb.TapPct(button.Item1, button.Item2, this.CancellationToken); // Select potions
                                await Task.Delay(2000, this.CancellationToken);
                                await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken);  // Use potion
                                await Task.Delay(2000, this.CancellationToken);
                                await Adb.FindButtonAndTap("#2060ce", 3000, 47, 57, 70, 5, this.CancellationToken);  // Potion used dialog
                                await Task.Delay(2000, this.CancellationToken);
                                await Adb.FindButtonAndTap("#2060ce", 3000, 50, 80, 90, 20, this.CancellationToken); // Enter button 2 again
                                await Task.Delay(2000, this.CancellationToken);

                            }
                            else
                            {
                                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Not enough stamina");
                                OnMachineFinished();
                                return;
                            }

                        }

                        // Confirm equipment box or enter
                        await Task.Delay(2000, this.CancellationToken);
                        if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for enter button 3");
                        if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                        {

                            // Enter if equipment confirmed, otherwise should find nothing
                            await Task.Delay(2000, this.CancellationToken);
                            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Checking for confirm equipment box");
                            if (await Adb.FindButtonAndTap("#2060ce", 3000, 61, 57, 70, 5, this.CancellationToken))
                            {
                                await Task.Delay(4000, this.CancellationToken);
                            }

                            // Reset state
                            await StateMachine.FireAsync(Trigger.ResetState);

                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 3");
                            OnMachineFinished();
                        }


                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 2");
                        OnMachineFinished();
                    }

                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find Enter button 1");
                    OnMachineFinished();
                }

            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to detect dungeon complete dialog");
                OnMachineFinished();
            }


        }

        private async Task RecoverCrash()
        {

            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash detected, attempting recovery!");

            // Go to home screen
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Navigating home...");
            await this.Adb.NavigateHome(this.CancellationToken);
            await Task.Delay(5000, this.CancellationToken);

            // Kill FFRK
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Kill ffrk process...");
            await this.Adb.StopPackage(Adb.FFRK_PACKAGE_NAME, this.CancellationToken);
            await Task.Delay(5000, this.CancellationToken);

            // Launch app
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Launching app");
            await this.Adb.StartActivity(Adb.FFRK_PACKAGE_NAME, Adb.FFRK_ACTIVITY_NAME, this.CancellationToken);
            await Task.Delay(5000, this.CancellationToken);

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
                        ConfigureStateMachine(State.Unknown);
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
                ConfigureStateMachine(State.Unknown);

                // Images to find
                List<Adb.ImageDef> items = new List<Adb.ImageDef>();
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_blue_play, Simalarity = 0.95f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_brown_ok, Simalarity = 0.95f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.lab_segment, Simalarity = 0.85f });

                // Stopwatch to limit how long we try to find buttons
                recoverStopwatch.Restart();

                // Button Finding Loop with timeout and break if stopwatch stopped
                double loopTimeout = TimeSpan.FromMinutes(3).TotalSeconds;
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Button finding loop for {0}s", loopTimeout);
                while (recoverStopwatch.Elapsed < TimeSpan.FromSeconds(loopTimeout) && recoverStopwatch.IsRunning)
                {
                    // Find images in order, breaking on first match
                    var ret = await Adb.FindImages(items, 3, this.CancellationToken);
                    if (ret != null)
                    {
                        // Tap it
                        await Adb.TapPct(ret.Item1, ret.Item2, this.CancellationToken);
                    }
                    // Delay between finds
                    await Task.Delay(5000, this.CancellationToken);
                }

                // Loop finshed, check state
                CancellationToken.ThrowIfCancellationRequested();
                recoverStopwatch.Stop();

                // Disable lab if timeout
                if (recoverStopwatch.Elapsed > TimeSpan.FromSeconds(loopTimeout))
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery timed out");
                    OnMachineFinished();
                    await Notify();
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery completed!");
                }

            }

        }

        private async Task RecoverFailed()
        {

            await Task.Delay(5000, this.CancellationToken);
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                await this.Adb.TapPct(50, 72, this.CancellationToken);
                await Task.Delay(2000, this.CancellationToken);
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

        }

    }
}
