using FFRK_LabMem.Data;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static FFRK_LabMem.Machines.Lab;

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

        public async Task ParseDisplayPaintings(JObject data, String url)
        {

            Lab.Data = data;

            // Status
            var status = data["labyrinth_dungeon_session"]["current_painting_status"];
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

        public async Task ParsePainting(JObject data, string url)
        {

            // Final portal completes dungeon
            if (data["labyrinth_dungeon_result"] != null)
            {
                await Lab.StateMachine.FireAsync(Trigger.FinishedLab);
                return;
            }

            // Data
            Lab.Data = data;

            // Event results
            var eventdata = data["labyrinth_dungeon_session"]["explore_painting_event"];
            var status = data["labyrinth_dungeon_session"]["current_painting_status"];

            // Data logging
            await DataLogger.LogExploreRate(Lab, eventdata, status, url.Contains("choose_explore_painting"));

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
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 10: // Fatigue
                        ParseAbrasionMap(data);
                        ColorConsole.WriteLine("Strayed into an area teeming with twisted memories");
                        await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 7:  // Door
                        await Lab.StateMachine.FireAsync(Trigger.FoundDoor);
                        break;
                    case 4:  // Battle
                        await Lab.StateMachine.FireAsync(Trigger.FoundBattle);
                        break;

                }
                return;
            }

            // Abrasion map presence
            if (ParseAbrasionMap(data))
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

            // Last buffs presence
            var lastAddonRM = data["labyrinth_dungeon_session"]["last_addon_record_materia"];
            if (lastAddonRM != null)
            {
                await Lab.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

        }

        public async Task ParsePartyList(JObject data, string url)
        {

            // If we have party data
            var parties = data["parties"];
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
                        Lab.FatigueInfo.Add(new List<BuddyInfo>());

                        foreach (JProperty item in party["slot_to_buddy_id"].Children<JProperty>().OrderBy(i => i.Name))
                        {
                            Lab.FatigueInfo[partySlot].Add(new BuddyInfo() { BuddyId = (int)item.Value });
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
            return true;
        }

        public async Task ParseFatigueInfo(JObject data, string url)
        {
            var values = (JArray)data["labyrinth_buddy_info"]["memory_abrasions"];
            if (values != null)
            {
                eventPartyList.WaitOne(TimeSpan.FromSeconds(10));
                foreach (var item in Lab.FatigueInfo.SelectMany(s => s).ToList())
                {
                    var value = (JObject)values.Where(i => (int)i["user_buddy_id"] == item.BuddyId).FirstOrDefault();
                    if (value != null) item.Fatigue = (int)value["memory_abrasion"];
                }
            }
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Fatigue values WRITE: {0}", Lab.AutoResetEventFatigue);
            Lab.AutoResetEventFatigue.Set();
            await Task.CompletedTask;
        }



        public async Task ParseAllData(JObject data, string url)
        {
            var info = data["labyrinth_dungeon"];
            if (info != null)
            {
                Counters.SetCurrentLab(info["node_id"].ToString(), info["name"].ToString());
                if (Lab.FinalFloor == 0)
                {
                    Lab.FinalFloor = (int)info["floor_num"];
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Final floor set to {0}", Lab.FinalFloor);
                }
            }
            await Task.CompletedTask;
        }

        public async Task ParseQEData(JObject data, string url)
        {

            var node = data["current_node"];
            if (node != null)
            {
                Lab.Data = data;
                await Counters.QuickExplore(node["id"].ToString(), node["name"].ToString());
                ColorConsole.WriteLine(ConsoleColor.Green, $"Quick Explore: {node["name"]}");
                await DataLogger.LogQEDrops(Lab);
                Counters.ClearCurrentLab();
                Lab.AutoResetEventQuickExplore.Set();
            }

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

    }
}
