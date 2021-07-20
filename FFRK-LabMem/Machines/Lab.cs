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
using System.Diagnostics;
using System.Timers;

namespace FFRK_LabMem.Machines
{
    public class Lab : Machine
    {

        public class Configuration
        {
            public bool Debug { get; set; }
            public bool OpenDoors { get; set; }
            public bool AvoidExploreIfTreasure { get; set; }
            public bool AvoidPortal { get; set; }
            public Dictionary<String, int> PaintingPriorityMap { get; set; }
            public Dictionary<String, int> TreasurePriorityMap { get; set; }
            public int MaxKeys {get; set;}
            public Point AppPosition { get; set; }
            public int BattleWatchdogMinutes { get; set; }
            public bool RestartFailedBattle { get; set; }

            public Configuration()
            {
                this.Debug = true;
                this.OpenDoors = true;
                this.AvoidExploreIfTreasure = true;
                this.AvoidPortal = true;
                this.MaxKeys = 3;
                this.BattleWatchdogMinutes = 10;
                this.RestartFailedBattle = false;
            }
        }

        public event EventHandler LabFinished;

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
            BattleCrashed,
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
            Finished,
            Crashed
        }

        public Adb Adb { get; set; }
        public StateMachine<State, Trigger> StateMachine { get; set; }
        public JObject Data { get; set; }
        public Configuration Config { get; set; }
        private Random rng = new Random();
        private Stopwatch battleStopwatch = new Stopwatch();
        private Timer battleWatchdogTimer = new Timer(System.Int32.MaxValue);

        public Lab(Adb adb, Configuration config)
        {

            // Config
            this.Config = config;
                        
            // Setup
            this.Adb = adb;
            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);

            // Timer
            if (this.Config.BattleWatchdogMinutes > 0)
            {
                battleWatchdogTimer.Interval = TimeSpan.FromMinutes(this.Config.BattleWatchdogMinutes).TotalMilliseconds;
                battleWatchdogTimer.Elapsed += battleWatchdogTimer_Elapsed;
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
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.BattleCrashed, State.Crashed)
                .Permit(Trigger.BattleFailed, State.Failed);

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
                .Permit(Trigger.MoveOn, State.Ready)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(t => SelectTreasures())
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(t => OpenSealedDoor())
                .Permit(Trigger.DontOpenDoor, State.FoundThing)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(t => EnterDungeon())
                .Permit(Trigger.EnterDungeon, State.EquipParty);

            this.StateMachine.Configure(State.EquipParty)
                .OnEntryAsync(t => StartBattle())
                .PermitReentry(Trigger.FoundBattle)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.BattleFailed, State.Failed)
                .Permit(Trigger.BattleCrashed, State.Crashed);

            this.StateMachine.Configure(State.BattleFinished)
                .OnEntryAsync(t => FinishBattle())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.PortalConfirm)
                .OnEntryAsync(t => ConfirmPortal())
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Finished)
                .OnEntryAsync(t => FinishLab())
                .Permit(Trigger.ResetState, State.Ready)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.BattleCrashed)
                .Ignore(Trigger.PickedCombatant);

            this.StateMachine.Configure(State.Crashed)
                .OnEntryAsync(t => RecoverCrash())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.StartBattle, State.Battle);

            this.StateMachine.Configure(State.Failed)
                .OnEntryAsync(t => RecoverFailed())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.StartBattle, State.Battle);
            
            // Console output
            if (this.Config.Debug) this.StateMachine.OnTransitioned((state) => { ColorConsole.WriteLine(ConsoleColor.DarkGray, "Entering state: {0}", state.Destination); });
            
            // Activate
            this.StateMachine.Fire(Trigger.Started);
            //string graph = UmlDotGraph.Format(this.StateMachine.GetInfo());

        }

        async void battleWatchdogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            await this.StateMachine.FireAsync(Trigger.BattleCrashed);
            this.battleWatchdogTimer.Stop();

        }
        
        public override void RegisterWithProxy(Proxy Proxy)
        {
            Proxy.AddRegistration("get_display_paintings", this);
            Proxy.AddRegistration("select_painting", this);
            Proxy.AddRegistration("choose_explore_painting", this);
            Proxy.AddRegistration("open_treasure_chest", this);
            Proxy.AddRegistration("dungeon_recommend_info", this);
            Proxy.AddRegistration("labyrinth/[0-9]+/win_battle", this);
            Proxy.AddRegistration("continue/get_info", this);
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
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
                            case 8:  // Portal
                                int floor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
                                ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", floor);
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
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
                    this.Data = data;
                    await this.StateMachine.FireAsync(Trigger.BattleSuccess);
                    break;

                case 6:
                    await this.StateMachine.FireAsync(Trigger.BattleFailed);
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
            int floor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
            var paintings = (JArray)this.Data["labyrinth_dungeon_session"]["display_paintings"];
            JToken selectedPainting = null;

            // New floor marker
            if (total == 20) ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", floor);

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

            // There's a treasure visible but picked a explore (unless last floor)
            // TODO: Determine if the last floor
            if (this.Config.AvoidExploreIfTreasure && isTreasure && (int)selectedPainting["type"] == 4 && (floor != 15 || floor !=20))
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

            // There's a treasure or explore visible or more paintings not visible yet, but picked a portal
            if (this.Config.AvoidPortal && (int)selectedPainting["type"] == 6 && (isTreasure || isExplore || (total > 9)))
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

            if ((bool)painting["is_special_effect"])
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Radiant painting detected: {0}", painting);
                return 0;
            }

            // Subtype for combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();
            }

            if (this.Config.PaintingPriorityMap.ContainsKey(type)){
                return this.Config.PaintingPriorityMap[type];
            } else {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Unknown painting id: {0}", type);
                return 99;
            }

        }

        private async Task SelectTreasures()
        {

            /*
             * 200001 = 6*, rainbow crystal
             * 300001 = 6* Mote, Key
             * 400001 = Anima Lens, Marker
             * 500103 = HE
             */

            // Got Item
            var i = this.Data["given_unsettled_items"];
            if (i != null)
            {
                foreach (var item in i)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Got Item: {0}", item["item_name"]);
                }
            }

            // Treasure list
            var treasures = (JArray)this.Data["labyrinth_dungeon_session"]["treasure_chest_ids"];

            // Already picked this many
            int picked = treasures.Where(t => (int)t == 0).Count();
            
            // Select a random treasure
            JToken treasureToPick = treasures
                .Select(t => t)
                .Where(t => GetTreasurePriority(t) > 0)
                .OrderBy(t => GetTreasurePriority(t))
                .ThenBy(t => rng.Next())
                .FirstOrDefault();

            // No treasures that match but we must pick one
            if (picked == 0 && treasureToPick == null)
                treasureToPick = treasures
                    .Select(t => t)
                    .OrderBy(t => rng.Next())
                    .FirstOrDefault();

            // Key check
            if (picked == 1 && this.Config.MaxKeys < 1) treasureToPick = null;
            if (picked == 2 && this.Config.MaxKeys < 3) treasureToPick = null;
            
            // Pick if we got something good
            if (treasureToPick != null) {

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

           }
           else
           {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                var b = await Adb.FindButtonAndTap(-14655282, 1000, 42.7, 62, 80, 10);
                if (b)
                {
                    await Task.Delay(1000);
                    if (picked != 3)
                    {
                        await this.Adb.TapPct(70, 64);
                        await Task.Delay(1000);
                    }
                    await this.StateMachine.FireAsync(Trigger.MoveOn);
                }

           }

        }

        private int GetTreasurePriority(JToken treasure)
        {

            var type = treasure.ToString().Substring(0,1);

            if (this.Config.TreasurePriorityMap.ContainsKey(type))
            {
                return this.Config.TreasurePriorityMap[type];
            }
            else
            {
                if (!type.Equals("0")) ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Unknown treasure id: {0}", type);
                return 0;
            }

        }

        private async Task OpenSealedDoor()
        {

            if (this.Config.OpenDoors)
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
                await this.StateMachine.FireAsync(Trigger.DontOpenDoor);
            }

        }

        private async Task MoveOn()
        {

            var i = this.Data["given_unsettled_items"];
            if (i != null)
            {
                foreach (var item in i)
	            {
                    ColorConsole.WriteLine(ConsoleColor.Green, "Got Item: {0}", item["item_name"]);
	            }
            }

            ColorConsole.WriteLine("Moving On...");
            await Task.Delay(5000);

            var b = await Adb.FindButtonAndTap(-14655282, 2000, 42.7, 69.4, 80.8, 30);
            if (b)
            {
                await Task.Delay(1000);
                await this.StateMachine.FireAsync(Trigger.MoveOn);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkBlue, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.ResetState);
            }
            
            // Failed

        }

        private async Task EnterDungeon()
        {
            ColorConsole.WriteLine("Enter Dungeon");
            await Task.Delay(2000);
            await this.Adb.TapPct(70, 86);
            await this.StateMachine.FireAsync(Trigger.EnterDungeon);
        }

        private async Task StartBattle()
        {
            ColorConsole.Write("Starting Battle");
            var d = this.Data["labyrinth_dungeon_session"]["dungeon"];
            if (d != null)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", d["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");
            
            var b = await Adb.FindButtonAndTap(-14655282, 2000, 42.7, 90, 95, 30);
            if (b)
            {
                await Task.Delay(500);
                await Adb.FindButtonAndTap(-14655282, 2000, 56, 60, 64, 5);
                await this.StateMachine.FireAsync(Trigger.StartBattle);
                battleStopwatch.Start();
                battleWatchdogTimer.Start();
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkBlue, "Failed to find button");
                await this.StateMachine.FireAsync(Trigger.ResetState);
            }
           
        }

        private async Task FinishBattle()
        {

            // Timer
            battleStopwatch.Stop();
            ColorConsole.Write("Battle Won!");
            ColorConsole.Write(ConsoleColor.DarkGray, " ({0:00}:{1:00})", battleStopwatch.Elapsed.Minutes, battleStopwatch.Elapsed.Seconds);
            battleStopwatch.Reset();
            battleWatchdogTimer.Stop();

            // Drops
            var r = this.Data["result"]["prize_master"];
            if (r != null)
            {
                foreach (var item in r)
                {
                    ColorConsole.WriteLine(ConsoleColor.Green, " Drop: {0}", item.First["name"]);
                }

            }

            //Tappy taps
            await Task.Delay(5000);
            await this.Adb.TapPct(85, 85);
            await Task.Delay(1000);
            await this.Adb.TapPct(50, 85);

        }

        private async Task ConfirmPortal()
        {

            await Task.Delay(2000);
            await this.Adb.TapPct(71, 62);
            await Task.Delay(2000);

        }

        private async Task FinishLab()
        {

            ColorConsole.WriteLine(ConsoleColor.Green, "We reached the master painting.  Press 'E' to enable when ready.");
            LabFinished(this, new EventArgs());
            // Notification?

            for (int i = 0; i < 5; i++)
            {
                Console.Beep();
                await Task.Delay(1000);
            }

        }

        private async Task RecoverCrash()
        {
            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash detected, attempting recovery!");
            await this.Adb.TapXY(this.Config.AppPosition.X, this.Config.AppPosition.Y);
            await Task.Delay(5000);
            var b = await Adb.FindButtonAndTap(-14655282, 2000, 40, 70, 83, 20);
            if (b)
            {
                if (await Adb.FindButtonAndTap(-14655282, 2000, 61, 57, 68, 20))
                    await this.StateMachine.FireAsync(Trigger.StartBattle);
            }

        }

        private async Task RecoverFailed()
        {

            await Task.Delay(5000);
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                battleWatchdogTimer.Stop();
                await this.Adb.TapPct(50, 72);
                await Task.Delay(2000);
                await this.Adb.TapPct(25, 55);
                battleWatchdogTimer.Start();
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Waiting for user input...");
                battleWatchdogTimer.Stop();
            }
            
            await this.StateMachine.FireAsync(Trigger.StartBattle);

        }

    }
}
