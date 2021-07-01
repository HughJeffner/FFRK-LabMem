using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;
using SharpAdbClient;
using Stateless;
using Stateless.Graph;

namespace FFRK_LabMem.Machines
{
    public class Lab : Machine
    {
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
            EnterBattle,
            BattleSuccess,
            BattleFailed
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
            Failed
        }

        public Adb Adb { get; set; }
        public StateMachine<State, Trigger> StateMachine { get; set; }
        public JObject Data { get; set; }
        private Random rng = new Random();

        public Lab(Adb adb)
        {

            this.Adb = adb;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);

            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState())
                .Permit(Trigger.ResetState, State.Ready)
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
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor);

            this.StateMachine.Configure(State.FoundThing)
                .OnEntryAsync(t => MoveOn())
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(t => PickTreasures())
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(t => OpenSealedDoor())
                .Permit(Trigger.DontOpenDoor, State.Ready)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(t => EnterBattle())
                .Permit(Trigger.EnterBattle, State.EquipParty);

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

            this.StateMachine.Fire(Trigger.Started);

            this.StateMachine.OnTransitioned((state) => { Console.WriteLine("Entering state: {0}", state.Destination); });
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
                    int total = (int)this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];

                    // Status
                    status = data["labyrinth_dungeon_session"]["current_painting_status"];
                    if (status != null && (int)status == 0 && total==20) // Fresh floor
                    {
                        await this.StateMachine.FireAsync(Trigger.ResetState);
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
                            case 1:
                            case 3:
                            case 5:
                            case 6:
                            case 10:
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
                            case 7:
                                await this.StateMachine.FireAsync(Trigger.FoundDoor);
                                break;
                            case 4:
                                await this.StateMachine.FireAsync(Trigger.FoundBattle);
                                break;

                        }
                    }
                   
                    break;

                case 3:
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
                .Take(3)
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])
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
           
            Console.WriteLine("Picking painting {0}", selectedPaintingIndex+1);
            await Task.Delay(10000);
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

        private int GetPaintingPriority(JToken s)
        {
            /*
             *  1	Combatant
             *  2	????
             *  3	Treasure
             *  4	Explore
             *  5	Onslaught
             *  6	Portal
             *  7	Restoration
             */

            switch ((int)s["type"]){
                case 3:
                    return 1;
                case 4:
                    return 2;
                case 1:
                    if ((int)s["display_type"] == 3) return 3;
                    if ((int)s["display_type"] == 2) return 7;
                    return 8;
                case 7:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                case 2:
                    return 9;
                default:
                    return 99;
            }
        }

        private async Task PickTreasures()
        {

            /*
             * 200001 = 6*, rainbow crystal
             * 300001 = 6* Mote, Key
             * 500103 = HE
             */

            // Pick treasure
            var treasures = (JArray)this.Data["labyrinth_dungeon_session"]["treasure_chest_ids"];
            int picked = treasures.Where(t => (int)t == 0).Count();

            do
            {

                // Select treasure
                JToken selectedTreasure = treasures
                    .Select(t => t)
                    .Where(t => (int)t >= 300001)
                    .FirstOrDefault();

                // Get selected treasure index and pick it
                int selectedTreasureIndex = 0;

                // Did we already pick 2 or select a valid treasure?, if not then move on
                if (selectedTreasure != null)
                {

                    selectedTreasureIndex = treasures.IndexOf(selectedTreasure);
                    await Task.Delay(1000);

                    // Click chest
                    Console.WriteLine("Picking treasure {0}", selectedTreasureIndex + 1);
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
                    Console.WriteLine("Moving On...");
                    await Task.Delay(1000);
                    await this.Adb.TapPct(50, 70);
                    await Task.Delay(1000);
                    await this.Adb.TapPct(70, 64);
                    await Task.Delay(1000);
                    this.StateMachine.Fire(Trigger.MoveOn);

                }

            } while (picked < 2);

        }

        private async Task OpenSealedDoor()
        {

            Console.WriteLine("Opening Door...");
            await Task.Delay(10000);
            await this.Adb.TapPct(70, 74);
            await Task.Delay(1000);

        }

        private async Task MoveOn()
        {
            Console.WriteLine("Moving On...");
            await Task.Delay(10000);
            await this.Adb.TapPct(50, 74);
            await Task.Delay(1000);
            this.StateMachine.Fire(Trigger.MoveOn);
        }

        private async Task EnterBattle()
        {
            Console.WriteLine("Enter Battle");
            await Task.Delay(2000);
            await this.Adb.TapPct(70, 86);
            this.StateMachine.Fire(Trigger.EnterBattle);
        }

        private async Task StartBattle()
        {
            Console.WriteLine("Starting Battle");
            await Task.Delay(10000);
            await this.Adb.TapPct(50, 93);
            await Task.Delay(250);
            var c = await this.Adb.GetPixelColorPct(56, 62);
            if (GetDistance(c, Color.FromArgb(-14655282)) < 1000) await this.Adb.TapPct(66, 62);
            this.StateMachine.Fire(Trigger.StartBattle);
        }

        private async Task FinishBattle()
        {

            Console.WriteLine("Battle Won!");
            await Task.Delay(5000);
            await this.Adb.TapPct(85, 85);
            await Task.Delay(1000);
            await this.Adb.TapPct(50, 85);

        }

        private async Task ConfirmPortal()
        {

            Console.WriteLine("Next Level");
            await Task.Delay(2000);
            await this.Adb.TapPct(71, 62);

        }

        private static int GetDistance(Color current, Color match)
        {
            int redDifference;
            int greenDifference;
            int blueDifference;

            redDifference = current.R - match.R;
            greenDifference = current.G - match.G;
            blueDifference = current.B - match.B;

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }
    
    }
}
