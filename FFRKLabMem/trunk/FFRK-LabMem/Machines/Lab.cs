using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using FFRK_LabMem.Extensions;
using Newtonsoft.Json.Linq;
using SharpAdbClient;
using Stateless;
using Stateless.Graph;

namespace FFRK_LabMem.Machines
{
    public class Lab : Machine
    {

        public enum LabPriorityStrategy
        {
            Balanced = 0,
            Quick = 1,
            Full = 2
        }

        public enum Trigger
        {
            Started,
            ResetState,
            FoundThing,
            FoundDoor,
            FoundBattle,
            FoundTreasure,
            PickedCombatant,
            PickedPortal,
            OpenDoor,
            DontOpenDoor,
            MoveOn,
            StartBattle,
            EnterDungeon,
            BattleSuccess,
            BattleFailed,
            FoundBoss
        }

        public enum State
        {
            Starting,
            Unknown,
            Ready,
            FoundThing,
            FoundTreasure,
            FoundSealedDoor,
            PortalConfirm,
            BattleInfo,
            EquipParty,
            Battle,
            BattleFinished,
            Failed,
            Finished
        }

        public Adb Adb { get; set; }
        public StateMachine<State, Trigger> StateMachine { get; set; }
        public JObject Data { get; set; }
        public Dictionary<String, int> PaintingPriorityMap { get; set; }
        public LabPriorityStrategy PriorityStrategy { get; set; }
        private Random rng = new Random();

        public Lab(Adb adb, LabPriorityStrategy priorityStrategy)
        {

            // Setup
            this.Adb = adb;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);
            this.PriorityStrategy = priorityStrategy;
            ColorConsole.WriteLine("Setting up Lab with priority: {0}", priorityStrategy);

            // painting priority map
            if (priorityStrategy == LabPriorityStrategy.Balanced)
            {
                this.PaintingPriorityMap = new Dictionary<string, int>(){
                   {"3", 1},   //Treasure
                   {"4", 2},   //Explore
                   {"7", 3},   //Restoration
                   {"5", 4},   //Onslaught
                   {"6", 5},   //Portal
                   {"1.3", 6}, //Red
                   {"1.2", 7}, //Orange
                   {"1.1", 8}, //Green
                   {"2", 9},   //Master
                };
            }
            else if (priorityStrategy == LabPriorityStrategy.Quick)
            {
                this.PaintingPriorityMap = new Dictionary<string, int>(){
                   {"3", 1},   //Treasure
                   {"7", 3},   //Restoration
                   {"5", 4},   //Onslaught
                   {"6", 5},   //Portal
                   {"4", 2},   //Explore
                   {"1.3", 6}, //Red
                   {"1.2", 7}, //Orange
                   {"1.1", 8}, //Green
                   {"2", 9},   //Master
                };
            }
            else
            {
                this.PaintingPriorityMap = new Dictionary<string, int>(){
                   {"3", 1},   //Treasure
                   {"4", 2},   //Explore
                   {"1.3", 3}, //Red
                   {"1.2", 4}, //Orange
                   {"1.1", 5}, //Green
                   {"7", 6},   //Restoration
                   {"5", 7},   //Onslaught
                   {"6", 8},   //Portal
                   {"2", 9},   //Master
                };
            }
            

            // State machine config
            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor)
                .Permit(Trigger.BattleSuccess, State.BattleFinished);

            this.StateMachine.Configure(State.Ready)
                .OnEntryAsync(t => SelectPainting())
                .PermitReentry(Trigger.ResetState)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.PickedCombatant, State.BattleInfo)
                .Permit(Trigger.PickedPortal, State.PortalConfirm)
                .Permit(Trigger.FoundBoss, State.Finished)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor);

            this.StateMachine.Configure(State.FoundThing)
                .OnEntryAsync(t => MoveOn())
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(t => SelectTreasures())
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(t => OpenSealedDoor())
                .Permit(Trigger.DontOpenDoor, State.Ready)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(t => EnterDungeon())
                .Permit(Trigger.EnterDungeon, State.EquipParty);

            this.StateMachine.Configure(State.EquipParty)
                .OnEntryAsync(t => StartBattle())
                .PermitReentry(Trigger.FoundBattle)
                .Permit(Trigger.StartBattle, State.Battle);

            this.StateMachine.Configure(State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.BattleFailed, State.Failed);

            this.StateMachine.Configure(State.BattleFinished)
                .OnEntryAsync(t => FinishBattle())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.PortalConfirm)
                .OnEntryAsync(t => ConfirmPortal())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Finished)
                .OnEntryAsync(t => FinishLab())
                .Ignore(Trigger.ResetState)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.PickedCombatant);
            
            // Console output
            this.StateMachine.OnTransitioned((state) => { ColorConsole.WriteLine(ConsoleColor.DarkGray, "Entering state: {0}", state.Destination); });
            
            // Activate
            this.StateMachine.Fire(Trigger.Started);
            //string graph = UmlDotGraph.Format(this.StateMachine.GetInfo());

        }
        
        public override void RegisterWithProxy(Proxy Proxy)
        {
            Proxy.AddRegistration("get_display_paintings", this);
            Proxy.AddRegistration("select_painting", this);
            Proxy.AddRegistration("choose_explore_painting", this);
            Proxy.AddRegistration("open_treasure_chest", this);
            Proxy.AddRegistration("dungeon_recommend_info", this);
            Proxy.AddRegistration("labyrinth/[0-9]+/win_battle", this);
        }

        public override async Task PassFromProxy(int id, string urlMatch, JObject data)
        {
            switch (id)
            {
                
                case 0:

                    this.Data = data;

                    // Status
                    var status = data["labyrinth_dungeon_session"]["current_painting_status"];
                    if (status != null && (int)status == 0)
                    {
                        await this.StateMachine.FireAsync(Trigger.ResetState);
                    }
                    if (status != null && (int)status == 1)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundDoor);
                    }
                    if (status != null && (int)status == 2)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                    }
                    if (status != null && (int)status == 3)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundBattle);
                    }
                    if (status != null && (int)status == 4)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                        break;
                    }
                    break;

                case 1:
                case 2:

                    this.Data = data;
                    int total = 0;
                    var t = this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
                    if (t != null) total = (int)t;

                    // Status
                    status = data["labyrinth_dungeon_session"]["current_painting_status"];
                    if (status != null && (int)status == 0 && total==20) // Fresh floor
                    {
                        await this.StateMachine.FireAsync(Trigger.ResetState);
                        break;
                    }
                    if (status != null && (int)status == 1)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundDoor);
                        break;
                    }
                    if (status != null && (int)status == 2)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                        break;
                    }
                    if (status != null && (int)status == 3)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundBattle);
                        break;
                    }
                    if (status != null && (int)status == 4)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                        break;
                    }

                    // Explore event result
                    var eventdata = data["labyrinth_dungeon_session"]["explore_painting_event"];
                    if (eventdata != null)
                    {
                        switch ((int)eventdata["type"])
                        {
                            case 1:  // Nothing
                            case 2:  // Item
                            case 3:  // Lab Item?
                            case 5:  // Spring

                            case 6:  // Buffs
                            case 8:  // Portal
                            case 10: // Fatigue
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
                            case 7:  // Door
                                await this.StateMachine.FireAsync(Trigger.FoundDoor);
                                break;
                            case 4:  // Battle
                                await this.StateMachine.FireAsync(Trigger.FoundBattle);
                                break;

                        }
                        break;
                    }

                    // Abrasion map presence
                    var abrasionMap = data["user_buddy_memory_abrasion_map"];
                    if (abrasionMap != null)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    }

                    // Last buffs presence
                    var lastAddonRM = data["labyrinth_dungeon_session"]["last_addon_record_materia"];
                    if (lastAddonRM != null)
                    {
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    }

                    break;

                case 3:
                    this.Data = data;
                    await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                    break;
        
                case 4:
                    await this.StateMachine.FireAsync(Trigger.PickedCombatant);
                    break;

                case 5:
                    await this.StateMachine.FireAsync(Trigger.BattleSuccess);
                    break;

                default:
                    System.Diagnostics.Debug.Print(data.ToString());
                    break;

            }
        }

        private void DetermineState()
        {
            
        }

        private async Task SelectPainting()
        {

            // Logic to determine painting
            int total = (int)this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
            var paintings = (JArray)this.Data["labyrinth_dungeon_session"]["display_paintings"];
            JToken selectedPainting = null;

            // Insert Priority Field
            foreach (var item in paintings)
            {
                item["priority"] = GetPaintingPriority(item);
            }

            // Is there a treasure vault or explore visible?
            var isTreasure = paintings.Any(p => (int)p["type"] == 3);
            var isExplore = paintings.Any(p => (int)p["type"] == 4);

            // Select top 1 priority from the first 3
            selectedPainting = paintings
                .Take(3)                            // Only from the first 3
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])   // Priority ordering
                .ThenBy(p => rng.Next())            // Random for matching priority
                .FirstOrDefault();

            // There's a treasure visible but picked a explore
            if (isTreasure && (int)selectedPainting["type"] == 4)
            {
                selectedPainting = paintings
                .Take(3)
                .Select(p => p)
                .Where(p => (int)p["type"] != 4)
                .OrderBy(p => (int)p["priority"])
                .FirstOrDefault();

                // No choice
                if (selectedPainting == null) selectedPainting = paintings[rng.Next(2)];

            }

            // There's a treasure or explore visible but picked a portal
            if ((isTreasure || isExplore) && (int)selectedPainting["type"] == 6)
            {
                selectedPainting = paintings
                .Take(3)
                .Select(p => p)
                .Where(p => (int)p["type"] != 6)
                .OrderBy(p => (int)p["priority"])
                .FirstOrDefault();
            }
                         
            // Get selected painting id
            int selectedPaintingIndex = 0;
            if (selectedPainting != null) selectedPaintingIndex = paintings.IndexOf(selectedPainting);
           
            // Master painting check
            if ((int)selectedPainting["type"] == 2)
            {
                await this.StateMachine.FireAsync(Trigger.FoundBoss);
                return;
            }

            // Do Pick
            ColorConsole.Write("Picking painting {0}: {1}", selectedPaintingIndex + 1, selectedPainting["name"]);
            if ((int)selectedPainting["type"] == 1)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", selectedPainting["dungeon"]["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");
            await Task.Delay(5000);
            
            // TODO: clean this painting placement handling up
            // 2 or less paintings remaining change position
            if (total >= 3)
            {
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50);
                await Task.Delay(1000);
                await this.Adb.TapPct(17 + (33 * (selectedPaintingIndex)), 50);
            }
            else if (total == 2)
            {
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50);
                await Task.Delay(1000);
                await this.Adb.TapPct(33 + (33 * (selectedPaintingIndex)), 50);
            }
            else
            {
                await this.Adb.TapPct(50, 50);
                await Task.Delay(1000);
                await this.Adb.TapPct(50, 50);
            }
            

            // Change state if needed
            //if (new List<int>() { 7, 5 }.Contains((int)selectedPainting["type"]))
            //{
            //    await this.StateMachine.FireAsync(Trigger.FoundThing);
            //}

            if ((int)selectedPainting["type"] == 6)
            {
                await this.StateMachine.FireAsync(Trigger.PickedPortal);
            }

        }

        private int GetPaintingPriority(JToken painting)
        {

            var type = painting["type"].ToString();

            // Subtype for combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();
            }

            if (this.PaintingPriorityMap.ContainsKey(type)){
                return this.PaintingPriorityMap[type];
            } else {
                return 99;
            }

        }

        private async Task SelectTreasures()
        {

            //TODO: The move on button is shifted downward when you take an item

            /*
             * 200001 = 6*, rainbow crystal
             * 300001 = 6* Mote, Key
             * 500103 = HE
             */

            // Treasure list
            var treasures = (JArray)this.Data["labyrinth_dungeon_session"]["treasure_chest_ids"];

            // Already picked this many
            int picked = treasures.Where(t => (int)t == 0).Count();

            // Select a random treasure
            JToken treasureToPick = treasures
                .Select(t => t)
                .Where(t => (int)t >= 300001)
                .OrderBy(t => rng.Next())
                .FirstOrDefault();

            // Pick if we got something good, limited to 2
            if (treasureToPick != null && picked < 2) {

                // Get item index
                int selectedTreasureIndex = 0;
                selectedTreasureIndex = treasures.IndexOf(treasureToPick);

                // Click chest
                ColorConsole.WriteLine("Picking treasure {0}", selectedTreasureIndex + 1);
                await Task.Delay(5000);
                await this.Adb.TapPct(17 + (33 * (selectedTreasureIndex)), 50);
                await Task.Delay(1000);

                // Check if key needed
                if (picked > 0)
                {
                    await this.Adb.TapPct(58, 44);
                    await Task.Delay(1000);
                }

                // Confirm
                await this.Adb.TapPct(70, 64);

                // Pick counter
                picked++;
           }
           else
           {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                var b = await Adb.FindButtonAndTap(-14655282, 1000, 42.7, 62, 80, 10);
                if (b)
                {
                    await Task.Delay(1000);
                    await this.Adb.TapPct(70, 64);
                    await Task.Delay(1000);
                    this.StateMachine.Fire(Trigger.MoveOn);
                }

           }

        }

        private async Task OpenSealedDoor()
        {

            if (this.PriorityStrategy != LabPriorityStrategy.Quick)
            {
                ColorConsole.WriteLine("Opening Door...");
                await Task.Delay(5000);
                await this.Adb.TapPct(70, 74);
                await Task.Delay(1000);
            }
            else
            {
                ColorConsole.WriteLine("Leaving Door...");
                await Task.Delay(5000);
                await this.Adb.TapPct(30, 74);
                await Task.Delay(1000);
            }

        }

        private async Task MoveOn()
        {
            ColorConsole.WriteLine("Moving On...");
            await Task.Delay(5000);

            var b = await Adb.FindButtonAndTap(-14655282, 1000, 42.7, 69.4, 80.8, 20);
            if (b)
            {
                await Task.Delay(1000);
                this.StateMachine.Fire(Trigger.MoveOn);
            }
            
            // Failed

        }

        private async Task EnterDungeon()
        {
            ColorConsole.WriteLine("Enter Dungeon");
            await Task.Delay(2000);
            await this.Adb.TapPct(70, 86);
            this.StateMachine.Fire(Trigger.EnterDungeon);
        }

        private async Task StartBattle()
        {
            ColorConsole.WriteLine("Starting Battle");
            var b = await Adb.FindButtonAndTap(-14655282, 2000, 50, 90, 95, 20);
            if (b)
            {
                await Task.Delay(500);
                await Adb.FindButtonAndTap(-14655282, 2000, 56, 60, 64, 5);
                this.StateMachine.Fire(Trigger.StartBattle);
            }
           
        }

        private async Task FinishBattle()
        {

            ColorConsole.WriteLine("Battle Won!");
            await Task.Delay(5000);
            await this.Adb.TapPct(85, 85);
            await Task.Delay(1000);
            await this.Adb.TapPct(50, 85);

        }

        private async Task ConfirmPortal()
        {

            ColorConsole.WriteLine("Next Level");
            await Task.Delay(2000);
            await this.Adb.TapPct(71, 62);

        }

        private async Task FinishLab()
        {

            ColorConsole.WriteLine(ConsoleColor.Green, "We reached the master painting.  Please restart the app for next time.");
            // Notification?

            for (int i = 0; i < 5; i++)
            {
                Console.Beep();
                await Task.Delay(1000);
            }

        }

    }
}
