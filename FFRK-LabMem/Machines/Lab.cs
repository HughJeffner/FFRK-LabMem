using System;
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
using System.Collections.Generic;
using FFRK_Machines.Threading;

namespace FFRK_LabMem.Machines
{
    public partial class Lab : Machine<Lab.State, Lab.Trigger, LabConfiguration>
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
            MoveOn,
            StartBattle,
            EnterDungeon,
            BattleSuccess,
            BattleFailed,
            WatchdogTimer,
            FoundBoss,
            MissedButton,
            FinishedLab,
            Restart
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
            Crashed,
            Restarting
        }
                
        private int CurrentKeys { get; set; }
        public JToken CurrentPainting { get; set; }
        public int CurrentFloor { get; set; }
        private readonly Stopwatch battleStopwatch = new Stopwatch();
        private readonly Stopwatch recoverStopwatch = new Stopwatch();
        private readonly Timer watchdogTimer = new Timer(Int32.MaxValue);
        private readonly AsyncAutoResetEvent fatigueParsedEvent = new AsyncAutoResetEvent(false);

        private class BuddyInfo
        {
            public int BuddyId { get; set; }
            public int Fatigue { get; set; } = 3;
        }

        private List<BuddyInfo> FatigueInfo = new List<BuddyInfo>();

        public Lab(Adb adb, LabConfiguration config)
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
                .Permit(Trigger.FoundDoor, State.FoundThing)
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
                .PermitReentry(Trigger.StartBattle)
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
                .Permit(Trigger.Restart, State.Restarting)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.WatchdogTimer);

            this.StateMachine.Configure(State.Restarting)
                .OnEntryAsync(async (t) => await RestartLab())
                .Permit(Trigger.ResetState, State.Unknown);

            this.StateMachine.Configure(State.WaitForBoss)
                .OnEntryAsync(async (t) => await FinishLab(t))
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.FinishedLab, State.Completed)
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
                    if (this.Data != null)
                    {
                        watchdogTimer.Start();
                        if (this.Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Watchdog kicked");
                    } else
                    {
                        if (this.Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Watchdog stopped");
                    }
                }
                recoverStopwatch.Stop();
                
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
            Proxy.AddRegistration("labyrinth/[0-9]+/get_battle_init_data", this);
            Proxy.AddRegistration("labyrinth/party/list", this);
            Proxy.AddRegistration("labyrinth/buddy/info", this);
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
                    await DataLogger.LogExploreRate(this, eventdata, status, id == 2);

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
                            case 6:  // Buffs
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
                            case 8:  // Portal
                                int floor = (int)this.Data["labyrinth_dungeon_session"]["current_floor"];
                                ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", floor);
                                await this.StateMachine.FireAsync(Trigger.FoundThing);
                                break;
                            case 5:  // Spring
                            case 10: // Fatigue
                                ParseAbrasionMap(data);
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
                    if (ParseAbrasionMap(data))
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
                    if (this.Data !=null) await this.StateMachine.FireAsync(Trigger.PickedCombatant);
                    break;

                case 5:
                    this.Data = data;
                    await this.StateMachine.FireAsync(Trigger.BattleSuccess);
                    break;

                case 6:
                    await this.StateMachine.FireAsync(Trigger.BattleFailed);
                    break;

                case 7:
                    recoverStopwatch.Stop();
                    break;
                case 8:
                    ParsePartyInfo(data);
                    break;
                case 9:
                    ParseFatigueInfo(data);
                    break;
                default:
                    System.Diagnostics.Debug.Print(data.ToString());
                    break;

            }
        }

        public override async Task Disable()
        {
            // Stop timers
            if (watchdogTimer.Enabled) watchdogTimer.Stop();
            if (this.Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Watchdog stopped");
            if (battleStopwatch.IsRunning)
            {
                battleStopwatch.Stop();
                battleStopwatch.Reset();
            }

            // Reset status
            this.Data = null;
            this.CurrentPainting = null;
            this.CurrentFloor = 0;
            this.CurrentKeys = 0;
            this.FatigueInfo.Clear();
            
            // Base
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

            // Parse Fatigue


        }

        private void ParsePartyInfo(JObject data)
        {

            var party = data["parties"].Where(p => (string)p["party_no"] == "1").FirstOrDefault();
            if (party != null)
            {
                
                FatigueInfo.Clear();
                foreach (JProperty item in party["slot_to_buddy_id"].Children<JProperty>().OrderBy(i => i.Name))
                {
                    FatigueInfo.Add(new BuddyInfo() { BuddyId = (int)item.Value });
                }
            }

        }

        private void ParseFatigueInfo(JObject data)
        {
            var values = (JArray)data["labyrinth_buddy_info"]["memory_abrasions"];
            if (values != null)
            {
                foreach (var item in FatigueInfo)
                {
                    var value = (JObject)values.Where(i => (int)i["user_buddy_id"] == item.BuddyId).FirstOrDefault();
                    if (value != null) item.Fatigue = (int)value["memory_abrasion"];

                }
            }
            if (Config.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Fatigue values WRITE: {0}", fatigueParsedEvent);
            fatigueParsedEvent.Set();
            
        }

        private bool ParseAbrasionMap(JObject data)
        {
            var map = data["user_buddy_memory_abrasion_map"];
            if (map == null) return false;
            foreach (var item in FatigueInfo)
            {
                var value = map[item.BuddyId.ToString()];
                if (value != null) item.Fatigue = (int)value["value"];
            }
            return true;
        }

    }

}
