using FFRK_LabMem.Data;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static FFRK_LabMem.Machines.Lab;
using static FFRK_Machines.Services.Proxy;

namespace FFRK_LabMem.Machines
{
    class LabParser
    {

        private Lab Lab;
        private readonly ManualResetEvent eventPartyList = new ManualResetEvent(true);

        public LabParser(Lab lab)
        {
            Lab = lab;
        }

        public async Task ParseDataChanged(JObject data)
        {

            // Parse number of keys & tears
            var labItems = (JArray)data["labyrinth_items"];
            if (labItems != null)
            {
                var keys = labItems.Where(i => i["labyrinth_item"]["id"].ToString().Equals("181000001")).FirstOrDefault();
                Lab.CurrentKeys = (int)(keys?["num"] ?? Lab.CurrentKeys);
                var tears = labItems.Where(i => i["labyrinth_item"]["id"].ToString().Equals("181000003")).FirstOrDefault();
                Lab.CurrentTears = (int)(tears?["num"] ?? Lab.CurrentTears);
                Debug.WriteLine("Keys:{0}, Tears:{1}", Lab.CurrentKeys, Lab.CurrentTears);
            }

            var unsettledItems = (JArray)data["unsettled_items"];
            if (unsettledItems != null && (labItems?.Count ?? 0) > 0)
            {
                var keys = unsettledItems.Where(i => i["item_id"].ToString().Equals("181000001")).FirstOrDefault();
                Lab.CurrentKeys += (int)(keys?["num"] ?? 0);
                var tears = unsettledItems.Where(i => i["item_id"].ToString().Equals("181000003")).FirstOrDefault();
                Lab.CurrentTears += (int)(tears?["num"] ?? 0);
                Debug.WriteLine("Keys2:{0}, Tears2:{1}", Lab.CurrentKeys, Lab.CurrentTears);
            }

            // Parse Floor
            var session = Lab.Data["labyrinth_dungeon_session"];
            if (session != null)
            {
                var floor = session["current_floor"];
                if (floor != null)
                {
                    int newFloor = (int)floor;
                    if (newFloor != Lab.CurrentFloor)
                    {
                        // New floor indicates we not in hang state
                        Lab.Watchdog.HangReset();

                        if (Lab.CurrentFloor != 0)
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", floor);
                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Starting on Floor {0}!", floor);
                        }

                        // Check if final floor
                        if (Lab.FinalFloor == 0)
                        {
                            if (await IsFinalFloor())
                            {
                                Lab.FinalFloor = newFloor;
                                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Final floor set to {0}", Lab.FinalFloor);
                            }
                        }

                    }
                    Lab.CurrentFloor = newFloor;
                }
            }

        }

        public async Task ParseDisplayPaintings(RegistrationHandlerArgs args)
        {

            // Handle error
            if (await CheckError(args.Data)) return;

            Lab.Data = args.Data;

            // Status
            var status = args.Data["labyrinth_dungeon_session"]["current_painting_status"];
            if (status != null && (int)status == 0)
            {
                await Lab.StateMachine.FireAsync(Trigger.ResetState);
            }
            if (status != null && (int)status == 1)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundDoor);
            }
            if (status != null && (int)status == 2)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundTreasure);
            }
            if (status != null && (int)status == 3)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundBattle);
            }
            if (status != null && (int)status == 4)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundTreasure);
            }

        }

        public async Task ParsePainting(RegistrationHandlerArgs args)
        {

            // Handle error
            if (await CheckError(args.Data)) return;

            // Final portal completes dungeon
            if (args.Data["labyrinth_dungeon_result"] != null)
            {
                await Lab.StateMachine.FireAsync(Trigger.FinishedLab);
                return;
            }

            // Data
            Lab.Data = args.Data;

            // Event results
            var eventdata = args.Data["labyrinth_dungeon_session"]["explore_painting_event"];
            var status = args.Data["labyrinth_dungeon_session"]["current_painting_status"];

            // Data logging
            await DataLogger.LogExploreRate(Lab, eventdata, status, args.Url.Contains("choose_explore_painting"));

            // Check status first
            if (status != null && (int)status == 0)
            {
                if (Lab.StateMachine.State == State.PortalConfirm)
                {
                    await Lab.StateMachine.FireAsync(Trigger.ResetState);
                    return;
                }

            }
            if (status != null && (int)status == 1)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundDoor);
                return;
            }
            if (status != null && (int)status == 2)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundTreasure);
                return;
            }
            if (status != null && (int)status == 3)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundBattle);
                return;
            }
            if (status != null && (int)status == 4)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundTreasure);
                return;
            }

            // Check explore event next
            if (eventdata != null)
            {
                switch ((int)eventdata["type"])
                {
                    case 1:  // Nothing
                        ColorConsole.WriteLine("Did not find anything");
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 2:  // Item
                    case 3:  // Lab Item?
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 6:  // Buffs
                        ColorConsole.WriteLine("Came across the statue of a gallant hero");
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 8:  // Portal
                        ColorConsole.WriteLine("Pulled into a portal painting!");
                        await Counters.PulledInPortal();
                        await Lab.StateMachine.FireAsync(Trigger.FoundPortal);
                        break;
                    case 5:  // Spring
                        ColorConsole.WriteLine("Discovered a mysterious spring");
                        ParseAbrasionMap(args.Data);
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 10: // Fatigue
                        ParseAbrasionMap(args.Data);
                        ColorConsole.WriteLine("Strayed into an area teeming with twisted memories");
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 7:  // Leave Door
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 4:  // Battle
                        await Lab.StateMachine.FireAsync(Trigger.FoundBattle);
                        break;

                }
                return;
            }

            // Abrasion map presence
            if (ParseAbrasionMap(args.Data))
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

            // Last buffs presence
            var lastAddonRM = args.Data["labyrinth_dungeon_session"]["last_addon_record_materia"];
            if (lastAddonRM != null)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

        }

        public async Task ParsePartyList(RegistrationHandlerArgs args)
        {

            // If we have party data
            var parties = args.Data["parties"];
            if (parties != null)
            {
                eventPartyList.Reset();
                Lab.FatigueInfo.Clear();

                // Loop through 3 parties
                for (int partySlot = 0; partySlot < 3; partySlot++)
                {
                    var party = parties.Where(p => (string)p["party_no"] == (partySlot + 1).ToString()).FirstOrDefault();
                    if (party != null)
                    {
                        Lab.FatigueInfo.Add(new LabFatigueInfo.BuddyInfoList(0));

                        foreach (JProperty item in party["slot_to_buddy_id"].Children<JProperty>().OrderBy(i => i.Name))
                        {
                            Lab.FatigueInfo[partySlot].Add(new LabFatigueInfo.BuddyInfo() { BuddyId = (int)item.Value });
                        }
                    }
                }
                eventPartyList.Set();
            }
            await Task.CompletedTask;

        }

        public bool ParseAbrasionMap(JObject data)
        {
            var map = data["user_buddy_memory_abrasion_map"];
            if (map == null) return false;
            eventPartyList.WaitOne(TimeSpan.FromSeconds(10));
            foreach (var item in Lab.FatigueInfo.SelectMany(s => s).ToList())
            {
                var value = map[item.BuddyId.ToString()];
                if (value != null) item.Fatigue = (int)value["value"];
            }
            Lab.FatigueInfo.Set("EVENT");
            return true;
        }

        public async Task ParseFatigueInfo(RegistrationHandlerArgs args)
        {
            var values = (JArray)args.Data["labyrinth_buddy_info"]["memory_abrasions"];
            if (values != null)
            {
                eventPartyList.WaitOne(TimeSpan.FromSeconds(10));
                foreach (var item in Lab.FatigueInfo.SelectMany(s => s).ToList())
                {
                    var value = (JObject)values.Where(i => (int)i["user_buddy_id"] == item.BuddyId).FirstOrDefault();
                    if (value != null) item.Fatigue = (int)value["memory_abrasion"];
                }
            }
            Lab.FatigueInfo.Set("PARTY");
            await Task.CompletedTask;
        }

        public Task<bool> ParseAllData(RegistrationHandlerArgs args)
        {
            if (args.Data == null) return Task.FromResult(false);

            // Dungeon info
            var info = args.Data["labyrinth_dungeon"];
            if (info != null)
            {
                Counters.SetCurrentLab(info["node_id"].ToString(), info["name"].ToString());
                int maxFloor = (int)info["floor_num"];
                if (Lab.FinalFloor != maxFloor)
                {
                    Lab.FinalFloor = maxFloor;
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Final floor set to {0}", Lab.FinalFloor);
                }
                Lab.StaminaInfo.Cost = (int)info["stamina"];
            }

            // Stamina info
            var user = args.Data["user"];
            if (user != null) Lab.StaminaInfo.SetStamina((int)user["stamina_recovery_remaining_time"], (int)user["max_stamina"]);
            
            // Potions info
            var potions = args.Data["user_stamina_recovery_agents"];
            if (potions != null) Lab.StaminaInfo.Potions = (int)potions[0]["num"];

            return Task.FromResult(true);
        }

        public async Task ParseQEData(RegistrationHandlerArgs args)
        {

            var node = args.Data["current_node"];
            if (node != null)
            {
                Lab.Data = args.Data;
                await Counters.QuickExplore(node["id"].ToString(), node["name"].ToString());
                ColorConsole.WriteLine(ConsoleColor.Green, $"Quick Explore: {node["name"]}");
                await DataLogger.LogQEDrops(Lab);
                Counters.ClearCurrentLab();
                Lab.AutoResetEventQuickExplore.Set();
            }

        }

        public async Task ParseEnterLab(RegistrationHandlerArgs args)
        {
            // Update or create fatigue info
            if (Lab.FatigueInfo.Count == 3)
            {
                // Set all units in all parties fatigue info to 3
                Lab.FatigueInfo.ForEach(p => p.ForEach(u => u.Fatigue = 3));

            } else
            {
                // Create default 3 parties, 5 units, with 3 fatigue
                Lab.FatigueInfo = new LabFatigueInfo(3, 5);
                
            }

            // Pre-set the downloaded signal
            Lab.FatigueInfo.Set("DEFAULT");

            // Reset current lab counters
            await Counters.Reset("CurrentLab", CounterSet.DataType.All);

            // Parse lab info
            await ParseAllData(args);

        }

        private async Task<bool> IsFinalFloor()
        {
            try
            {
                await Task.Delay(2000, Lab.CancellationToken);
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking if final floor");
                return (await Lab.Adb.FindButton("#75377a", 2000, 48.6, 23, 24, 0, Lab.CancellationToken) != null);
            }
            catch (OperationCanceledException) { };
            return false;
        }

        private async Task<bool> CheckError(JObject data)
        {
            if (data["error"] != null)
            {
                await Lab.HandleError();
                return true;
            }
            return false;
        }
    }
}
