using FFRK_LabMem.Data;
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
using FFRK_Machines.Services.Adb;

namespace FFRK_LabMem.Machines
{
    public partial class Lab : Machine<Lab.State, Lab.Trigger, LabConfiguration>
    {

        private const string BUTTON_BLUE = "#2060ce";
        private const string BUTTON_BROWN = "#6c3518";
        private const string BUTTON_ORANGE = "#c85f07";
        private const string BUTTON_SKIP = "#d4d8f6";

        private readonly Dictionary<string, string> Combatant_Color = new Dictionary<string, string> { 
            {"1","G" },
            {"2", "O" },
            {"3", "R" }
        };
 
        private async Task DelayedTapPct(string key, double X, double Y)
        {
            await LabTimings.Delay(key, this.CancellationToken);
            await this.Adb.TapPct(X, Y, this.CancellationToken);
        }

        private async Task<bool> DelayedTapButton(string key, string color, int threshold, double X, double Y1, double Y2, int retries, double granularity = 0.5, int certainty = 0)
        {

            // Initial delay
            await LabTimings.Delay(key, this.CancellationToken);

            // Find
            var ret = await this.Adb.FindButtonAndTap(color, threshold, X, Y1, Y2, retries, this.CancellationToken, granularity, certainty);

            // Tuning
            LabTimings.TuneTiming(key, ret.tapped, ret.retries);
            
            return ret.tapped;
        }

        private async Task<Tuple<double,double>> DelayedFindButton(string key, string color, int threshold, double X, double Y1, double Y2, int retries)
        {
            // Initial delay
            await LabTimings.Delay(key, this.CancellationToken);

            // Find
            var ret = await this.Adb.FindButton(color, threshold, X, Y1, Y2, retries, this.CancellationToken);

            if (ret == null) return null;
            return ret.button;
        }

        private async Task DetermineState()
        {
            if (!Config.AutoStart || this.Data != null) return;
            await AutoStart();
        }

        private async Task AutoStart()
        {

            try
            {
                await LabTimings.Delay("Pre-AutoStart", this.CancellationToken);
                ColorConsole.WriteLine(ConsoleColor.DarkGray, "Trying to auto-start");

                // Images to find
                List<Adb.ImageDef> items = new List<Adb.ImageDef>();
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_inventory, Simalarity = 0.90f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.button_skip, Simalarity = 0.90f });
                items.Add(new Adb.ImageDef() { Image = Properties.Resources.lab_segment, Simalarity = 0.85f });

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
                        await Adb.NavigateBack(this.CancellationToken);
                    }

                    // Skip button
                    if (ret.Equals(items[1]))
                    {
                        await MoveOnFromBattle();
                    }

                    // Lab segment
                    if (ret.Equals(items[2]))
                    {
                        await Adb.TapPct(ret.Location.Item1, ret.Location.Item2, this.CancellationToken);
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
            if ((int)this.CurrentPainting["type"] == 2 && (Config.StopOnMasterPainting || Config.UseTeleportStoneOnMasterPainting))
            {
                var needsMission = Config.CompleteDailyMission == LabConfiguration.CompleteMissionOption.DefeatMasterPainting && !Counters.IsMissionCompleted();
                if (Config.UseTeleportStoneOnMasterPainting && needsMission && !Config.StopOnMasterPainting)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Defeating master painting for daily mission");
                } else
                {
                    await this.StateMachine.FireAsync(Trigger.FoundBoss);
                    return;
                }
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
           
            // Tap painting
            // 2 or less paintings remaining change position
            int offset = (total>=2)?33:0;
            int margin = 17;
            if (total == 2) margin = 33;
            if (total == 1) margin = 50;
            var target = margin + (offset * selectedPaintingIndex);

            // Spam taps if on minitouch
            if (Adb.Input == Adb.InputType.Minitouch)
            {
                // Delay, then spam taps for duration
                await LabTimings.Delay("Pre-SelectPainting", this.CancellationToken);
                await Adb.TapPctSpam(target, 50, await LabTimings.GetTimeSpan("Inter-SelectPainting"), this.CancellationToken);
            } else
            {
                // Delayed tap, then delayed tap again
                await DelayedTapPct("Pre-SelectPainting", target, 50);
                await DelayedTapPct("Inter-SelectPainting", target, 50);
            }
           
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

        private async Task SelectTreasures(StateMachine<State, Trigger>.Transition transition)
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
                if (transition.Source == State.Unknown || transition.Source == State.FoundSealedDoor) await Task.Delay(3000); // Additional delay if from explore/unknown
                await DelayedTapPct("Pre-SelectTreasure", 17 + (33 * (selectedTreasureIndex)), 50);

                // Check if key needed
                if (picked > 0)
                {
                    ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Magic Key] x{0} of {1}", picked, CurrentKeys);
                    this.CurrentKeys -= picked;
                    await DelayedTapButton("Inter-SelectTreasure", BUTTON_BROWN, 4000, 58, 40, 50, 10);
                }

                // Confirm
                await DelayedTapButton("Inter-SelectTreasure", BUTTON_BLUE, 4000, 70, 60, 70, 10);
                await Counters.UsedKeys(picked);
                await Counters.TreasureOpened();

            }
            else
            {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                if (await DelayedTapButton("Inter-SelectTreasure", BUTTON_BLUE, 4000, 40, 62, 80, 10))
                {
                    if (picked != 3)
                    {
                        await DelayedTapButton("Inter-SelectTreasure", BUTTON_BLUE, 4000, 60, 58, 68, 10);
                    }
                    await this.StateMachine.FireAsync(Trigger.MoveOn);
                }

            }

            await LabTimings.Delay("Post-SelectTreasure", this.CancellationToken);

        }

        private async Task OpenSealedDoor()
        {
            ColorConsole.WriteLine("{0} Door...", this.Config.OpenDoors ? "Opening" : "Leaving");
            bool foundButton;
            if (this.Config.OpenDoors)
            {
                foundButton = await DelayedTapButton("Pre-Door", BUTTON_BLUE, 4000, 70, 66, 80, 10);
            }
            else
            {
                foundButton = await DelayedTapButton("Pre-Door", BUTTON_BROWN, 4000, 30, 66, 80, 10);
            }
            if (!foundButton)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await AutoStart();
            }
            await LabTimings.Delay("Post-Door", this.CancellationToken);

        }

        private async Task MoveOn(bool FromPortal)
        {

            await DataLogger.LogGotItem(this);
            ColorConsole.WriteLine("Moving On...");

            if (await DelayedTapButton("Pre-MoveOn", BUTTON_BLUE, 4000, 42.7, 65, 81, 30))
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
            ColorConsole.WriteLine("Battle Info");
            bool button = false;
            if (Config.PartyIndex == LabConfiguration.PartyIndexOption.InstaBattle)
            {
                button = await DelayedTapButton("Pre-BattleInfo", BUTTON_ORANGE, 1250, 13.8, 77, 93, 25, 0.5, 1);
            } else
            {
                button = await DelayedTapButton("Pre-BattleInfo", BUTTON_BLUE, 2000, 59, 77, 93, 25, 0.5, 1);
                if (button) await this.StateMachine.FireAsync(Trigger.EnterDungeon);
            }

            if(!button)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.MissedButton);
            }

        }

        private async Task StartBattle()
        {
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
            var needsLetheTears = Config.UseLetheTears && (!Config.LetheTearsMasterOnly || (int)(this.CurrentPainting?["type"] ?? 0) == 2); // Using tears AND Not MasterOnly option or a master painting
            var needsPartyIndex = Config.PartyIndex == LabConfiguration.PartyIndexOption.LowestFatigueAny || Config.PartyIndex == LabConfiguration.PartyIndexOption.LowestFatigue12;
            if (needsLetheTears || needsPartyIndex)
            {
                // Wait for fatigue values downloaded on another thread
                var gotFatigueValues = await AutoResetEventFatigue.WaitAsync(await LabTimings.GetTimeSpan("Pre-StartBattle-Fatigue"), this.CancellationToken);
                if (gotFatigueValues)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Fatigue values READ: {0}", AutoResetEventFatigue);

                    // Select the party index with fatigue levels available
                    await SelectParty(selector.GetPartyIndex());

                    // Fatigue level check for tears
                    if (needsLetheTears &&
                        SelectedPartyIndex < FatigueInfo.Count && 
                        FatigueInfo[SelectedPartyIndex].Any(f => 
                            (Config.LetheTearsSlots[SelectedPartyIndex] & (1 << 4 - FatigueInfo[SelectedPartyIndex].IndexOf(f))) != 0 && 
                            f.Fatigue >= Config.LetheTearsFatigue
                        )
                    ) await UseLetheTears();
   
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Timed out waiting for fatigue values");
                }
                
            } else
            {
                // Just select the party Index
                await SelectParty(selector.GetPartyIndex());
            }

            // Enter
            if (await DelayedTapButton("Pre-StartBattle", BUTTON_BLUE, 3000, 42.7, 85, 95, 30))
            {
                // Fatigue warning
                await DelayedTapButton("Inter-StartBattle", BUTTON_BLUE, 2000, 56, 55, 65, 5, 0.5, 1);
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

            // Delay then select
            await DelayedTapPct("Pre-SelectParty", 50, (index == 1) ? 50 : 66.7);
            await LabTimings.Delay("Post-SelectParty", this.CancellationToken);
        }

        private async Task FinishBattle()
        {

            // Timer
            battleStopwatch.Stop();
            ColorConsole.Write("Battle Won!");
            ColorConsole.Write(ConsoleColor.DarkGray, @" ({0:mm\:ss})", battleStopwatch.Elapsed);
            await Counters.BattleWon(battleStopwatch.Elapsed);
            battleStopwatch.Reset();
            Watchdog.BattleReset();

            // Drops
            await DataLogger.LogBattleDrops(this);

            // Update fatigue unknown value
            if (SelectedPartyIndex < FatigueInfo.Count) FatigueInfo[SelectedPartyIndex].ForEach(f => f.Fatigue = -1);
            AutoResetEventFatigue.Reset();

            // Check if safe disable requested
            if (await CheckDisableSafeRequested()) return;

            // Tap through the results screen
            await MoveOnFromBattle();

        }

        private async Task MoveOnFromBattle()
        {

            // Wait for skip button
            if (await DelayedTapButton("Post-Battle", BUTTON_SKIP, 3000, 85, 80, 90, 10, 0.2))
            {
                 // Tappy taps
                if (Adb.Input == Adb.InputType.Minitouch)
                {
                    // Spam taps for duration
                    await Adb.TapPctSpam(50, 85, await LabTimings.GetTimeSpan("Post-BattleButton"), this.CancellationToken);
                }
                else
                {
                    // Delayed tap, then delayed tap again
                    await DelayedTapPct("Post-BattleButton", 50, 85);
                }

                // Check if we defeated the boss
                if (this.Data != null && this.Data["result"] != null && this.Data["result"]["labyrinth_dungeon_result"] != null)
                    await this.StateMachine.FireAsync(Trigger.FinishedLab);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Did not find skip button!");
            }

        }

        private async Task ConfirmPortal()
        {

            await DelayedTapButton("Pre-ConfirmPortal", BUTTON_BLUE, 3000, 86, 58, 68, 10, 0.5, 1);
            await LabTimings.Delay("Post-ConfirmPortal", this.CancellationToken);
        }

        private async Task<bool> UseLetheTears()
        {

            int numberUsed = Convert.ToString(Config.LetheTearsSlots[SelectedPartyIndex], 2).ToCharArray().Count(c => c == '1');

            // Check remaining qty
            if (numberUsed > CurrentTears){
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough lethe tears!");
                return true;
            }

            ColorConsole.WriteLine(ConsoleColor.Magenta, "Using [Lethe Tears] x{0} of {1}", numberUsed, CurrentTears);

            // Lethe tears button
            await DelayedTapPct("Pre-LetheTears", 88.88, 17.18);

            // Each unit if selected
            var partyY = 32.33333 + (16.66666 * SelectedPartyIndex);
            for (int i = 0; i < 5; i++)
            {
                if ((Config.LetheTearsSlots[SelectedPartyIndex] & (1 << 4-i)) != 0)
                {
                    await DelayedTapPct("Inter-LetheTears-Unit", 11.11 + (i * 15.55), partyY);
                }
            }

            // Confirm button
            if (await DelayedTapButton("Inter-LetheTears", BUTTON_BLUE, 3000, 37.5, 74, 87, 20))
            {
                //Use Lethe Tears brown button
                if (await DelayedTapButton("Inter-LetheTears", BUTTON_BROWN, 2000, 50, 29, 42, 20))
                {
                    // Confirmation
                    if (await DelayedTapButton("Inter-LetheTears", BUTTON_BLUE, 3000, 61, 57, 70, 5))
                    {
                        // OK
                        if (await DelayedTapButton("Inter-LetheTears", BUTTON_BLUE, 3000, 38.8, 55, 70, 5))
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

            // Lethe tears button
            await DelayedTapPct("Pre-TeleportStone", 90.27, 4.68);

            // Use a stone brown button
            if (await DelayedTapButton("Inter-TeleportStone", BUTTON_BROWN, 2000, 58, 23, 37.5, 20))
            {
                // Confirmation
                if (await DelayedTapButton("Inter-TeleportStone", BUTTON_BLUE, 3000, 61, 57, 70, 5))
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

            // Ending on master painting not defeated
            if (t.Destination == State.WaitForBoss)
            {
                ColorConsole.WriteLine(ConsoleColor.Green, "We reached the master painting.  ");

                // Stop on master painting takes priority
                if (Config.StopOnMasterPainting)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Press 'E' to enable when ready.");
                    await Notify(Notifications.EventType.LAB_COMPLETED, "Stopped on master painting");
                    await Counters.LabRunCompleted(false);
                    base.OnMachineFinished();
                } else if (Config.UseTeleportStoneOnMasterPainting)
                {
                    await UseTeleportStone();
                    await StateMachine.FireAsync(Trigger.FinishedLab);
                }
                
            }

            if (t.Destination == State.Completed)
            {

                // Message
                ColorConsole.Write(ConsoleColor.Green, "Lab run completed!");
                ColorConsole.WriteLine(ConsoleColor.DarkGray, @" ({0:h\:mm\:ss})", Counters.Default.CounterSets["CurrentLab"].Runtime["Total"]);

                // Notify complete (only if not restarting)
                if (t.Source != State.Unknown)
                {
                    await Notify(Notifications.EventType.LAB_COMPLETED, "Lab run completed successfully");
                    await Counters.LabRunCompleted(t.Source == State.BattleFinished || t.Source == State.PortalConfirm);
                }

                // Mission
                var needsMission = Config.CompleteDailyMission == LabConfiguration.CompleteMissionOption.QuickExplore && !Counters.IsMissionCompleted();
                if (Config.UseTeleportStoneOnMasterPainting && needsMission)
                {
                    await CompleteMissionByQE();
                }

                // Restart or not
                if (!Config.RestartLab)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Press 'E' to enable when ready.");
                    base.OnMachineFinished();
                }
                else
                {
                    // Check restart counter
                    if (RestartLabCounter != 0)
                    {
                        if (RestartLabCounter >= 0) ColorConsole.WriteLine(ConsoleColor.Green, "{0} Restart(s) remaining", RestartLabCounter);
                        RestartLabCounter -= 1;
                        await this.StateMachine.FireAsync(Trigger.Restart);
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Green, "Maximum number of restarts reached");
                        base.OnMachineFinished();
                    }
                    
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
            var closeButton = await DelayedFindButton("Inter-RestartLab", BUTTON_BROWN, 2000, 39, 81, 91, 10);
            if (closeButton != null)
            {
                await Adb.TapPct(closeButton.Item1, closeButton.Item2, this.CancellationToken);
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Dungeon complete dialog not present");
            }

            // Mission Complete
            Watchdog.Kick(true);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for mission complete dialog");
            await DelayedTapButton("Inter-RestartLab", BUTTON_BROWN, 2000, 39, 61, 82, 5);
            
            // Enter button 1
            Watchdog.Kick(true);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 1");
            if (await DelayedTapButton("Inter-RestartLab", BUTTON_BLUE, 3000, 50, 84, 94, 20))
            {

                // Enter button 2
                Watchdog.Kick(true);
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 2");
                if (await DelayedTapButton("Inter-RestartLab", BUTTON_BLUE, 3000, 50, 80, 90, 20))
                {

                    // Stamina dialog
                    Watchdog.Kick(true);
                    var staminaResult = await CheckRestoreStamina();
                    if (staminaResult.PotionUsed)
                    {
                        // Enter button 2 again
                        await DelayedTapButton("Inter-RestartLab", BUTTON_BLUE, 3000, 50, 80, 90, 20); 
                    }
                    else
                    {
                        if (staminaResult.StaminaDialogPresent) return;
                    }
   
                    // Confirm equipment box or enter
                    Watchdog.Kick(true);
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for enter button 3");
                    if (await DelayedTapButton("Inter-RestartLab", BUTTON_BLUE, 3000, 61, 57, 70, 5))
                    {

                        // Enter if equipment confirmed, otherwise should find nothing
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for confirm equipment box");
                        await DelayedTapButton("Inter-RestartLab", BUTTON_BLUE, 3000, 61, 57, 70, 5);

                        // Delay
                        await LabTimings.Delay("Post-RestartLab", this.CancellationToken);

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
                    if (await Adb.FindButton(BUTTON_BROWN, 3000, 40.2, 88.3, 97.7, 3, this.CancellationToken) != null)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Inventory full!");
                        await Notify(Notifications.EventType.LAB_FAULT, "Inventory full");
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
                    await Adb.TapPct(staminaButton.button.Item1, staminaButton.button.Item2, this.CancellationToken);

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

            // Use potion
            if (await DelayedTapButton("Pre-StaminaPotion", BUTTON_BLUE, 3000, 61, 60, 80, 5))
            {
                
                // Confirm qty
                if (await DelayedTapButton("Inter-StaminaPotion", BUTTON_BLUE, 3000, 61, 55, 75, 5)){

                    await Counters.UsedStaminaPot();

                    // Potion used dialog
                    if (await DelayedTapButton("Inter-StaminaPotion", BUTTON_BLUE, 3000, 47, 55, 70, 5))
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
            await LabTimings.Delay("Inter-RestartFFRK", this.CancellationToken);

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

            // Stopwatch to limit how long we try to find buttons
            recoverStopwatch.Restart();

            // Button Finding Loop with timeout and break if stopwatch stopped
            TimeSpan loopTimeout = await LabTimings.GetTimeSpan("Inter-RestartFFRK-Timeout");
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Button finding loop for {0}s", loopTimeout.TotalSeconds);
            while (recoverStopwatch.Elapsed < loopTimeout && recoverStopwatch.IsRunning)
            {
                // Find images in order, breaking on first match
                var ret = await Adb.FindImages(items, 3, this.CancellationToken);
                if (ret != null)
                {
                    // Tap it
                    await Adb.TapPct(ret.Location.Item1, ret.Location.Item2, this.CancellationToken);
                }
                // Delay between finds
                await Task.Delay(Adb.CaptureRate, this.CancellationToken);
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
            }
            await LabTimings.Delay("Post-RestartFFRK", this.CancellationToken);
            return true;

        }

        private async Task RestartBattle()
        {
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                if (Watchdog.BattleFailed())
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                    await DelayedTapPct("Pre-RestartBattle", 50, 72);
                    await DelayedTapPct("Inter-RestartBattle", 25, 55);
                    await CheckAutoBattle();
                }
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Waiting for user input...");
                await Notify(Notifications.EventType.LAB_FAULT, "Battle failed");
                Watchdog.Kick(false);
            }
            await LabTimings.Delay("Post-RestartBattle", this.CancellationToken);

        }

        public async Task<bool> QuickExplore()
        {
           
            // Dungeon Complete
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Dismissing complete dialog");
            var closeButton = await DelayedFindButton("Pre-QuickExplore", BUTTON_BROWN, 2000, 39, 81, 91, 3);
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
            var quickExploreButton = await DelayedFindButton("Inter-QuickExplore", BUTTON_BROWN, 2000, 82.6, 83.2, 93.7, 10);
            if (quickExploreButton != null)
            {
                // Tap It
                await Adb.TapPct(quickExploreButton.Item1, quickExploreButton.Item2, CancellationToken);

                // Use a record marker button
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing use a record marker");
                var useRecordMarkerButton = await DelayedFindButton("Inter-QuickExplore", BUTTON_BROWN, 2000, 50, 25.7, 36.3, 2);
                
                // If record marker button not present then stamina too low
                if (useRecordMarkerButton == null)
                {

                    // Stamina Check
                    await LabTimings.Delay("Inter-QuickExplore", this.CancellationToken);
                    var staminaResult = await CheckRestoreStamina();
                    if (staminaResult.PotionUsed)
                    {
                        // Quick Explore Button Again
                        await DelayedTapPct("Inter-QuickExplore", quickExploreButton.Item1, quickExploreButton.Item2);

                        // Look for the record marker button again
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing use a record marker");
                        useRecordMarkerButton = await DelayedFindButton("Inter-QuickExplore", BUTTON_BROWN, 2000, 50, 25.7, 39, 2);
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
                    if (await DelayedTapButton("Inter-QuickExplore", BUTTON_BLUE, 2000, 64, 54.6, 70.3, 5))
                    {

                        // Stamina Cost
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Choosing stamina cost OK button");
                        if (await DelayedTapButton("Inter-QuickExplore", BUTTON_BLUE, 2000, 64, 54, 70.3, 5))
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

        private async Task CheckAutoBattle()
        {

            await LabTimings.Delay("Pre-CheckAutoBattle", this.CancellationToken);
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking for auto-battle");
            List<Adb.ImageDef> items = new List<Adb.ImageDef>
            {
                new Adb.ImageDef() { Image = Properties.Resources.battle_commands, Simalarity = 0.90f }
            };

            for (int i = 0; i < 9; i++)
            {
                var ret = await Adb.FindImages(items, 3, this.CancellationToken);
                if (ret != null)
                {
                    // Tap it
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Auto-battle disabled, attempting to enable it now!");
                    await Adb.TapPct(9.1, 77.2, CancellationToken);
                    return;
                } else
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Did not find disabled auto-battle state");
                }
                await Task.Delay(Adb.CaptureRate, this.CancellationToken);
            }
            await LabTimings.Delay("Post-CheckAutoBattle", this.CancellationToken);

        }

        private async Task<bool> CompleteMissionByQE()
        {
            Watchdog.Kick(false); // Pause the watchdog
            ColorConsole.WriteLine(ConsoleColor.Green, "Doing Quick Explore for daily mission");
            ColorConsole.WriteLine("Waiting for 60 seconds");
            await Task.Delay(30000);
            ColorConsole.WriteLine("Waiting for 30 seconds");
            await Task.Delay(20000);
            ColorConsole.WriteLine("Waiting for 10 seconds");
            await Task.Delay(10000);
            ColorConsole.WriteLine(ConsoleColor.Green, "Starting Quick Explore");
            var ret = await QuickExplore();
            Watchdog.Kick(); // Resume the watchdog
            return ret;

        }

        private async Task EnterOutpost()
        {

            await DelayedTapPct("Pre-EnterOutpost", 50, 40);
            if (Config.RestartLab) await StateMachine.FireAsync(Trigger.FinishedLab);

        }

        public async Task HandleError()
        {
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Connection error, retrying....");
            await DelayedTapButton("Pre-HandleError", BUTTON_BLUE, 2000, 39.5, 54, 67.5, 10, 0.5, 1);
        }

    }
}
