using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;
using Stateless;
using System.Diagnostics;
using System.Timers;
using FFRK_Machines.Machines;
using FFRK_Machines;
using FFRK_LabMem.Data;

namespace FFRK_LabMem.Machines
{
    public class Lab : Machine<Lab.State, Lab.Trigger, Lab.Configuration>
    {

        public class Configuration : MachineConfiguration
        {
            public bool OpenDoors { get; set; }
            public bool AvoidExploreIfTreasure { get; set; }
            public bool AvoidPortal { get; set; }
            public Dictionary<String, int> PaintingPriorityMap { get; set; }
            public Dictionary<String, int> TreasurePriorityMap { get; set; }
            public int MaxKeys {get; set;}
            public int WatchdogMinutes { get; set; }
            public bool RestartFailedBattle { get; set; }
            public bool StopOnMasterPainting { get; set; }

            public Configuration()
            {
                this.Debug = true;
                this.OpenDoors = true;
                this.AvoidExploreIfTreasure = true;
                this.AvoidPortal = true;
                this.MaxKeys = 3;
                this.WatchdogMinutes = 10;
                this.RestartFailedBattle = false;
                this.StopOnMasterPainting = true;
            }
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
            WatchdogTimer,
            FoundBoss,
            MissedButton,
            FinishedLab
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
            WaitForBoss,
            Completed,
            Crashed
        }
                
        private int CurrentKeys { get; set; }
        private JToken CurrentPainting { get; set; }
        private Stopwatch battleStopwatch = new Stopwatch();
        private Timer watchdogTimer = new Timer(System.Int32.MaxValue);

        public Lab(Adb adb, Configuration config)
        {

            // Config
            this.Config = config;
            this.Adb = adb;
           
            // Timer
            if (this.Config.WatchdogMinutes > 0)
            {
                watchdogTimer.AutoReset = false;
                watchdogTimer.Interval = TimeSpan.FromMinutes(this.Config.WatchdogMinutes).TotalMilliseconds;
                watchdogTimer.Elapsed += battleWatchdogTimer_Elapsed;
            }

            // State machine
            ConfigureStateMachine(State.Starting);
                       
            // Activate
            this.StateMachine.Fire(Trigger.Started);

            // Debug graph
            //string graph = UmlDotGraph.Format(this.StateMachine.GetInfo());
            
        }

        public override void ConfigureStateMachine(State initialState)
        {

            this.StateMachine = new StateMachine<State, Trigger>(initialState);
            
            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(t => DetermineState())
                .Ignore(Trigger.WatchdogTimer)
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.PickedCombatant, State.BattleInfo)
                .Permit(Trigger.BattleFailed, State.Failed)
                .Permit(Trigger.FoundBoss, State.WaitForBoss)
                .Permit(Trigger.FinishedLab, State.Completed);

            this.StateMachine.Configure(State.Ready)
                .OnEntryAsync(async (t) => await SelectPainting())
                .PermitReentry(Trigger.ResetState)
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.PickedCombatant, State.BattleInfo)
                .Permit(Trigger.PickedPortal, State.PortalConfirm)
                .Permit(Trigger.FoundBoss, State.WaitForBoss)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor)
                .Permit(Trigger.FinishedLab, State.Completed);

            this.StateMachine.Configure(State.FoundThing)
                .OnEntryAsync(async (t) => await MoveOn())
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.MoveOn, State.Ready)
                .Permit(Trigger.MissedButton, State.Ready);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(async (t) => await SelectTreasures())
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(async (t) => await OpenSealedDoor())
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.DontOpenDoor, State.FoundThing)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(async (t) => await EnterDungeon())
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.EnterDungeon, State.EquipParty)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.EquipParty)
                .OnEntryAsync(async (t) => await StartBattle())
                .PermitReentry(Trigger.FoundBattle)
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.StartBattle, State.Battle)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.BattleFailed, State.Failed)
                .Permit(Trigger.WatchdogTimer, State.Crashed);

            this.StateMachine.Configure(State.BattleFinished)
                .OnEntryAsync(async (t) => await FinishBattle())
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.FinishedLab, State.Completed)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.PortalConfirm)
                .OnEntryAsync(async (t) => await ConfirmPortal())
                .Permit(Trigger.FinishedLab, State.Completed)
                .Permit(Trigger.WatchdogTimer, State.Crashed)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Completed)
                .OnEntryAsync(async (t) => await FinishLab(t))
                .Permit(Trigger.ResetState, State.Ready)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.WatchdogTimer);

            this.StateMachine.Configure(State.WaitForBoss)
                .OnEntryAsync(async (t) => await FinishLab(t))
                .Permit(Trigger.ResetState, State.Ready)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.WatchdogTimer)
                .Ignore(Trigger.PickedCombatant);

            this.StateMachine.Configure(State.Crashed)
                .OnEntryAsync(async (t) => await RecoverCrash())
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.StartBattle, State.Battle);

            this.StateMachine.Configure(State.Failed)
                .OnEntryAsync(async (t) => await RecoverFailed())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .PermitReentry(Trigger.BattleFailed);

            base.ConfigureStateMachine(initialState);

            if (Config.WatchdogMinutes > 0) StateMachine.OnTransitioned((state) => { 
                if (state.Trigger != Trigger.WatchdogTimer)
                {
                    watchdogTimer.Stop();
                    watchdogTimer.Start();
                }
                
            });


        }

        async void battleWatchdogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            await this.StateMachine.FireAsync(Trigger.WatchdogTimer);

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

                    // Final portal completes dungeon
                    if (data["labyrinth_dungeon_result"] != null)
                    {
                        await this.StateMachine.FireAsync(Trigger.FinishedLab);
                        break;
                    }

                    // Data
                    this.Data = data;
                    int remainingPaintings = 0;
                    var remainingData = this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
                    if (remainingData != null) remainingPaintings = (int)remainingData;

                    // Event results
                    var eventdata = data["labyrinth_dungeon_session"]["explore_painting_event"];
                    status = data["labyrinth_dungeon_session"]["current_painting_status"];

                    // Data logging
                    await DataLogger.LogExploreResult(eventdata, status, this.CurrentPainting, id == 2);

                    // Check status first
                    if (status != null && (int)status == 0) // Fresh floor
                    {
                        if (remainingPaintings == 20 || this.StateMachine.State == State.PortalConfirm)
                        {
                            await this.StateMachine.FireAsync(Trigger.ResetState);
                            break;
                        }
                        
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
                                     
                    // Check explore event next
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

        public override async Task Disable()
        {
            if (watchdogTimer.Enabled) watchdogTimer.Stop();
            if (battleStopwatch.IsRunning)
            {
                battleStopwatch.Stop();
                battleStopwatch.Reset();
            }
            await base.Disable();
        }

        protected override void OnDataChanged(JObject data)
        {

            // Parse number of keys
            var labItems = (JArray)data["labyrinth_items"];
            if (labItems != null)
            {
                var keys = labItems.Where(i => i["labyrinth_item"]["id"].ToString().Equals("181000001")).FirstOrDefault();
                if (keys != null)
                {
                    this.CurrentKeys = (int)keys["num"];
                }
            }
            var unsettledItems = (JArray)data["unsettled_items"];
            if (unsettledItems != null)
            {
                var keys = unsettledItems.Where(i => i["item_id"].ToString().Equals("181000001")).FirstOrDefault();
                if (keys != null)
                {
                    this.CurrentKeys+= (int)keys["num"];
                }
            }

        }

        #region "State Actions"

        private void DetermineState()
        {

        }

        private async Task SelectPainting()
        {

            // Logic to determine painting
            int total = (int)this.Data["labyrinth_dungeon_session"]["remaining_painting_num"];
            int floor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
            var paintings = (JArray)this.Data["labyrinth_dungeon_session"]["display_paintings"];
            this.CurrentPainting = null;

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
            this.CurrentPainting = paintings
                .Take(3)                            // Only from the first 3
                .Select(p => p)
                .OrderBy(p => (int)p["priority"])   // Priority ordering
                .ThenBy(p => rng.Next())            // Random for matching priority
                .FirstOrDefault();

            // There's a treasure visible but picked a explore (unless last floor)
            // TODO: Determine if the last floor
            if (this.Config.AvoidExploreIfTreasure && isTreasure && (int)this.CurrentPainting["type"] == 4 && floor != 15 && floor !=20)
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
            if ((int)this.CurrentPainting["type"] == 2 && this.Config.StopOnMasterPainting)
            {
                await this.StateMachine.FireAsync(Trigger.FoundBoss);
                return;
            }

            // Do Pick
            ColorConsole.Write("Picking painting {0}: {1}", selectedPaintingIndex + 1, this.CurrentPainting["name"]);
            if ((int)this.CurrentPainting["type"] == 1)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", this.CurrentPainting["dungeon"]["captures"][0]["tip_battle"]["title"]);
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
            await DataLogger.LogGotItem(this.Data, this.CurrentPainting);

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
            if ((picked == 1 && this.CurrentKeys == 0) || 
                (picked == 2 && this.CurrentKeys < 2))
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Not enough keys!");
                treasureToPick = null;
            }

            // Pick if we got something good
            if (treasureToPick != null) {

                // Get item index
                int selectedTreasureIndex = 0;
                selectedTreasureIndex = treasures.IndexOf(treasureToPick);

                // Click chest
                ColorConsole.WriteLine("Picking treasure {0}", selectedTreasureIndex + 1);
                await Task.Delay(5000);
                await this.Adb.TapPct(17 + (33 * (selectedTreasureIndex)), 50);
                await Task.Delay(2000);

                // Check if key needed
                if (picked > 0)
                {
                    await this.Adb.TapPct(58, 44);
                    await Task.Delay(2000);
                }

                // Confirm
                await this.Adb.TapPct(70, 64);

           }
           else
           {

                // Move On
                ColorConsole.WriteLine("Moving On...");
                var b = await Adb.FindButtonAndTap("#2060ce", 4000, 40, 62, 80, 10);
                if (b)
                {
                    await Task.Delay(2000);
                    if (picked != 3)
                    {
                        await this.Adb.TapPct(70, 64);
                        await Task.Delay(2000);
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

            await DataLogger.LogGotItem(this.Data, this.CurrentPainting);
            ColorConsole.WriteLine("Moving On...");
            await Task.Delay(5000);

            var b = await Adb.FindButtonAndTap("#2060ce", 4000, 42.7, 65, 81, 30);
            if (b)
            {
                await Task.Delay(1000);
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
            var b = await Adb.FindButtonAndTap("#2060ce", 2000, 56.6, 80, 95, 30);
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
            var d = this.Data["labyrinth_dungeon_session"]["dungeon"];
            if (d != null)
            {
                ColorConsole.Write(": ");
                ColorConsole.Write(ConsoleColor.Yellow, "{0}", d["captures"][0]["tip_battle"]["title"]);
            }
            ColorConsole.WriteLine("");
            
            if (await Adb.FindButtonAndTap("#2060ce", 3000, 42.7, 85, 95, 30))
            {
                await Task.Delay(500);
                await Adb.FindButtonAndTap("#2060ce", 2000, 56, 55, 65, 5);
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
            await DataLogger.LogBattleDrops(this.Data, this.CurrentPainting);
            
            //Tappy taps
            await Task.Delay(6000);
            await this.Adb.TapPct(85, 85);
            await Task.Delay(1000);
            await this.Adb.TapPct(50, 85);

            // Check if we defeated the boss
            if (this.Data["result"]["labyrinth_dungeon_result"] != null) 
                await this.StateMachine.FireAsync(Trigger.FinishedLab);


        }

        private async Task ConfirmPortal()
        {

            await Task.Delay(5000);
            await this.Adb.TapPct(71, 62);
            await Task.Delay(2000);

        }

        private async Task FinishLab(StateMachine<State,Trigger>.Transition t)
        {

            // Disable machine
            base.OnMachineFinished();

            if (t.Destination == State.WaitForBoss)
                ColorConsole.WriteLine(ConsoleColor.Green, "We reached the master painting.  Press 'E' to enable when ready.");

            if (t.Destination == State.Completed)
                ColorConsole.WriteLine(ConsoleColor.Green, "Lab run completed!  Press 'E' to enable when ready.");

            // Notification?
            await Notify();

        }

        private async Task RecoverCrash()
        {

            const String FFRK_PACKAGE_NAME = "com.dena.west.FFRK";
            const String FFRK_ACTIVITY_NAME = "jp.dena.dot.Dot";

            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash detected, attempting recovery!");

            // Go to home screen
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Navigating home...");
            await this.Adb.NavigateHome();
            await Task.Delay(5000);

            // Kill FFRK
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Kill ffrk process...");
            await this.Adb.StopPackage(FFRK_PACKAGE_NAME);
            await Task.Delay(5000);

            // Launch app
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Launching app");
            await this.Adb.StartActivity(FFRK_PACKAGE_NAME, FFRK_ACTIVITY_NAME);
            await Task.Delay(5000);

            // Press start button
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Wating for start button...");
            if (await Adb.FindButtonAndTap("#2060ce", 4000, 40, 70, 83, 20))
            {
                // Press continue battle button
                if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Wating for continue battle button...");
                if (await Adb.FindButtonAndTap("#2060ce", 4000, 61, 57, 68, 10))
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery restarted battle");
                    await this.StateMachine.FireAsync(Trigger.StartBattle);
                } else
                {
                    // Go back into lab
                    if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Tapping lab...");
                    await this.Adb.TapPct(50, 50);
                    ColorConsole.WriteLine(ConsoleColor.DarkRed, "Crash recovery entered lab");
                    ConfigureStateMachine(State.Unknown);
                }

            } else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Failed to detect FFRK restart");
                OnMachineFinished();
                await Notify();
            }

        }

        private async Task RecoverFailed()
        {

            await Task.Delay(5000);
            ColorConsole.Write(ConsoleColor.DarkRed, "Battle failed! ");
            if (this.Config.RestartFailedBattle)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restarting...");
                await this.Adb.TapPct(50, 72);
                await Task.Delay(2000);
                await this.Adb.TapPct(25, 55);
                await this.StateMachine.FireAsync(Trigger.StartBattle);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Waiting for user input...");
                await Notify();
                watchdogTimer.Stop();
            }
            
        }

    #endregion

    }

}
