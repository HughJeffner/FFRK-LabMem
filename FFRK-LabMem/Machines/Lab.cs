using System;
using System.Linq;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;
using Stateless;
using System.Diagnostics;
using FFRK_Machines.Machines;
using FFRK_Machines;
using FFRK_LabMem.Data;
using System.Collections.Generic;
using FFRK_Machines.Threading;
using FFRK_Machines.Services.Notifications;

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
            FoundPortal,
            PickedCombatant,
            PickedPortal,
            OpenDoor,
            MoveOn,
            StartBattle,
            EnterDungeon,
            BattleSuccess,
            BattleFailed,
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
            Restarting
        }

        private int CurrentKeys { get; set; }
        public JToken CurrentPainting { get; set; }
        public int CurrentFloor { get; set; }
        public int FinalFloor { get; set; }
        public LabWatchdog Watchdog { get; }
        private bool disableSafeRequested = false;
        private readonly Stopwatch battleStopwatch = new Stopwatch();
        private readonly Stopwatch recoverStopwatch = new Stopwatch();
        private readonly AsyncAutoResetEvent fatigueAutoResetEvent = new AsyncAutoResetEvent(false);
        private int restartTries = 0;

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
            this.Watchdog = new LabWatchdog(adb, config.WatchdogHangMinutes, config.WatchdogCrashSeconds);
            this.Watchdog.Timeout += Watchdog_Timeout;

        }

        public override void ConfigureStateMachine()
        {

            this.StateMachine = new StateMachine<State, Trigger>(State.Starting);
            
            this.StateMachine.Configure(State.Starting)
                .Permit(Trigger.Started, State.Unknown);

            this.StateMachine.Configure(State.Unknown)
                .OnEntry(async (t) => await DetermineState())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundPortal, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.PickedCombatant, State.BattleInfo)
                .Permit(Trigger.BattleFailed, State.Failed)
                .Permit(Trigger.FoundBoss, State.WaitForBoss)
                .Permit(Trigger.FinishedLab, State.Completed);

            this.StateMachine.Configure(State.Ready)
                .OnEntryAsync(async (t) => await SelectPainting())
                .PermitReentry(Trigger.ResetState)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundPortal, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.PickedCombatant, State.BattleInfo)
                .Permit(Trigger.PickedPortal, State.PortalConfirm)
                .Permit(Trigger.FoundBoss, State.WaitForBoss)
                .Permit(Trigger.FoundDoor, State.FoundSealedDoor)
                .Permit(Trigger.FinishedLab, State.Completed);

            this.StateMachine.Configure(State.FoundThing)
                .OnEntryFromAsync(Trigger.FoundThing, async (t) => await MoveOn(false))
                .OnEntryFromAsync(Trigger.FoundDoor, async (t) => await MoveOn(false))
                .OnEntryFromAsync(Trigger.FoundPortal, async(t) => await MoveOn(true))
                .Permit(Trigger.MoveOn, State.Ready)
                .Permit(Trigger.MissedButton, State.Unknown);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(async (t) => await SelectTreasures())
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(async (t) => await OpenSealedDoor())
                .Permit(Trigger.FoundDoor, State.FoundThing)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(async (t) => await EnterDungeon())
                .Permit(Trigger.EnterDungeon, State.EquipParty)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.EquipParty)
                .OnEntryAsync(async (t) => await StartBattle())
                .PermitReentry(Trigger.FoundBattle)
                .Permit(Trigger.StartBattle, State.Battle)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.Battle)
                .OnEntry((t) => battleStopwatch.Restart())
                .PermitReentry(Trigger.StartBattle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .Permit(Trigger.BattleFailed, State.Failed);

            this.StateMachine.Configure(State.BattleFinished)
                .OnEntryAsync(async (t) => await FinishBattle())
                .Permit(Trigger.FinishedLab, State.Completed)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.PortalConfirm)
                .OnEntryAsync(async (t) => await ConfirmPortal())
                .Permit(Trigger.FinishedLab, State.Completed)
                .Permit(Trigger.ResetState, State.Ready);

            this.StateMachine.Configure(State.Completed)
                .OnEntryAsync(async (t) => await FinishLab(t))
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.Restart, State.Restarting)
                .Ignore(Trigger.BattleSuccess);

            this.StateMachine.Configure(State.Restarting)
                .OnEntryAsync(async (t) => await RestartLab())
                .Permit(Trigger.ResetState, State.Unknown);

            this.StateMachine.Configure(State.WaitForBoss)
                .OnEntryAsync(async (t) => await FinishLab(t))
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.FinishedLab, State.Completed)
                .Ignore(Trigger.BattleSuccess)
                .Ignore(Trigger.PickedCombatant);

            this.StateMachine.Configure(State.Failed)
                .OnEntryAsync(async (t) => await RestartBattle())
                .Permit(Trigger.ResetState, State.Ready)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.BattleSuccess, State.BattleFinished)
                .PermitReentry(Trigger.BattleFailed);

            base.ConfigureStateMachine();

            // Start machine
            StateMachine.FireAsync(Trigger.Started);

            // Watchdog
            StateMachine.OnTransitioned((state) => {
                Watchdog.Kick(this.Data != null);
                recoverStopwatch.Stop();
            });

        }

        private async void Watchdog_Timeout(object sender, LabWatchdog.WatchdogEventArgs e)
        {

            // Ignore hang if in battle
            if (e.Type == LabWatchdog.WatchdogEventArgs.TYPE.Hang && StateMachine.State == State.Battle)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Ignoring hang because in battle");
                return;
            }

            ColorConsole.WriteLine(ConsoleColor.DarkRed, "{0} detected!", e.Type);

            // On a timer thread, need to handle errors
            bool result = false;
            try
            {
                // Restart ffrk and get result
                result = await RestartFFRK();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
            }

            // Restart watchdog if failed
            if (!result)
            {
                // Limit number of retries
                if (restartTries < Config.WatchdogMaxRetries)
                {
                    restartTries += 1;
                    ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Starting watchdog after failed FFRK restart (try {0})", restartTries);
                    Watchdog.Kick();
                } else
                {
                    // Retries exhausted
                    await Notify(Notifications.EventType.LAB_FAULT);
                    OnMachineFinished();
                }
            } else
            {
                restartTries = 0;
            }

        }

        public override void RegisterWithProxy(Proxy Proxy)
        {
            Proxy.AddRegistration("get_display_paintings", Handle_GetDisplayPaintings);
            Proxy.AddRegistration("select_painting", Handle_Painting);
            Proxy.AddRegistration("choose_explore_painting", Handle_Painting);
            Proxy.AddRegistration("open_treasure_chest", async (data, url) =>
            {
                this.Data = data;
                await this.StateMachine.FireAsync(Trigger.FoundTreasure);
            });
            Proxy.AddRegistration("dungeon_recommend_info", async(data, url) => 
            { 
                if (this.Data != null) await this.StateMachine.FireAsync(Trigger.PickedCombatant); 
            });
            Proxy.AddRegistration("labyrinth/[0-9]+/win_battle", async(data, url) => 
            {
                this.Data = data;
                await this.StateMachine.FireAsync(Trigger.BattleSuccess);
            });
            Proxy.AddRegistration("continue/get_info", async(data, url) =>
            {
                await this.StateMachine.FireAsync(Trigger.BattleFailed);
            });
            Proxy.AddRegistration("labyrinth/[0-9]+/get_battle_init_data", async(data, url) => {
                recoverStopwatch.Stop();
                await this.StateMachine.FireAsync(Trigger.StartBattle);
            }) ;
            Proxy.AddRegistration("labyrinth/party/list", ParsePartyInfo);
            Proxy.AddRegistration("labyrinth/buddy/info", ParseFatigueInfo);
            Proxy.AddRegistration(@"/dff/\?timestamp=[0-9]+", ParseAllData);
            Proxy.AddRegistration("labyrinth/[0-9]+/do_simple_explore", ParseQEData);
        }

        private async Task Handle_GetDisplayPaintings(JObject data, String url)
        {

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
            }

        }

        private async Task Handle_Painting(JObject data, string url)
        {

            // Final portal completes dungeon
            if (data["labyrinth_dungeon_result"] != null)
            {
                await this.StateMachine.FireAsync(Trigger.FinishedLab);
                return;
            }

            // Data
            this.Data = data;

            // Event results
            var eventdata = data["labyrinth_dungeon_session"]["explore_painting_event"];
            var status = data["labyrinth_dungeon_session"]["current_painting_status"];

            // Data logging
            await DataLogger.LogExploreRate(this, eventdata, status, url.Contains("choose_explore_painting"));

            // Check status first
            if (status != null && (int)status == 0)
            {
                if (this.StateMachine.State == State.PortalConfirm)
                {
                    await this.StateMachine.FireAsync(Trigger.ResetState);
                    return;
                }

            }
            if (status != null && (int)status == 1)
            {
                await this.StateMachine.FireAsync(Trigger.FoundDoor);
                return;
            }
            if (status != null && (int)status == 2)
            {
                await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                return;
            }
            if (status != null && (int)status == 3)
            {
                await this.StateMachine.FireAsync(Trigger.FoundBattle);
                return;
            }
            if (status != null && (int)status == 4)
            {
                await this.StateMachine.FireAsync(Trigger.FoundTreasure);
                return;
            }

            // Check explore event next
            if (eventdata != null)
            {
                switch ((int)eventdata["type"])
                {
                    case 1:  // Nothing
                        ColorConsole.WriteLine("Did not find anything");
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 2:  // Item
                    case 3:  // Lab Item?
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 6:  // Buffs
                        ColorConsole.WriteLine("Came across the statue of a gallant hero");
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 8:  // Portal
                        ColorConsole.WriteLine("Pulled into a portal painting!");
                        await Counters.PulledInPortal();
                        await this.StateMachine.FireAsync(Trigger.FoundPortal);
                        break;
                    case 5:  // Spring
                        ColorConsole.WriteLine("Discovered a mysterious spring");
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 10: // Fatigue
                        ParseAbrasionMap(data);
                        ColorConsole.WriteLine("Strayed into an area teeming with twisted memories");
                        await this.StateMachine.FireAsync(Trigger.FoundThing);
                        break;
                    case 7:  // Door
                        await this.StateMachine.FireAsync(Trigger.FoundDoor);
                        break;
                    case 4:  // Battle
                        await this.StateMachine.FireAsync(Trigger.FoundBattle);
                        break;

                }
                return;
            }

            // Abrasion map presence
            if (ParseAbrasionMap(data))
            {
                await this.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

            // Last buffs presence
            var lastAddonRM = data["labyrinth_dungeon_session"]["last_addon_record_materia"];
            if (lastAddonRM != null)
            {
                await this.StateMachine.FireAsync(Trigger.FoundThing);
                return;
            }

        }

        public void DisableSafe()
        {
            disableSafeRequested = true;
        }
        
        protected override void OnEnabled()
        {
            Watchdog.Enable();
            disableSafeRequested = false;
        }

        protected override void OnDisabled()
        {
            // Stop timers
            Watchdog.Disable();
            if (battleStopwatch.IsRunning)
            {
                battleStopwatch.Stop();
                battleStopwatch.Reset();
            }

            // Reset status
            this.Data = null;
            this.CurrentPainting = null;
            this.CurrentFloor = 0;
            this.FinalFloor = 0;
            this.CurrentKeys = 0;
            this.FatigueInfo.Clear();
            fatigueAutoResetEvent.Reset();
            disableSafeRequested = false;

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

            // Parse Floor
            var session = this.Data["labyrinth_dungeon_session"];
            if (session != null)
            {
                var floor = session["current_floor"];
                if (floor != null)
                {
                    int newFloor = (int)floor;
                    if (newFloor != CurrentFloor)
                    {
                        if (CurrentFloor != 0)
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Welcome to Floor {0}!", floor);
                        } else
                        {
                            ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Starting on Floor {0}!", floor);
                        }

                        // Check if final floor
                        if (this.FinalFloor == 0)
                        {
                            if (IsFinalFloor().Result)
                            {
                                this.FinalFloor = newFloor;
                                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Final floor set to {0}", this.FinalFloor);
                            }
                        }

                    }
                    this.CurrentFloor = newFloor;
                }
            }

        }

        private async Task ParsePartyInfo(JObject data, string url)
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
            await Task.CompletedTask;

        }

        private async Task ParseFatigueInfo(JObject data, string url)
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
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Fatigue values WRITE: {0}", fatigueAutoResetEvent);
            fatigueAutoResetEvent.Set();
            await Task.CompletedTask;
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

        private async Task ParseAllData(JObject data, string url)
        {
            var info = data["labyrinth_dungeon"];
            if (info != null)
            {
                Counters.SetCurrentLab(info["node_id"].ToString(), info["name"].ToString());
                if (this.FinalFloor == 0)
                {
                    this.FinalFloor = (int)info["floor_num"];
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Final floor set to {0}", this.FinalFloor);
                }
            }
            await Task.CompletedTask;
        }

        private async Task ParseQEData(JObject data, string url)
        {

            var node = data["current_node"];
            if (node != null)
            {
                Counters.ClearCurrentLab();
                Counters.SetCurrentLab(node["id"].ToString(), node["name"].ToString());
                this.Data = data;
                await DataLogger.LogQEDrops(this);
                Counters.ClearCurrentLab();
            }
            await Task.CompletedTask;


        }

        private async Task<bool> IsFinalFloor()
        {
            try
            {
                await Task.Delay(2000, this.CancellationToken);
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Checking if final floor");
                return (await Adb.FindButton("#75377a", 2000, 48.6, 23, 24, 0, this.CancellationToken) != null);

            } catch (OperationCanceledException){};
            return false;
        }

        private Task<bool> CheckDisableSafeRequested()
        {
            if (disableSafeRequested)
            {
                OnMachineFinished();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task ManualFFRKRestart(bool showMessage = true)
        {
            if (showMessage) ColorConsole.WriteLine(ConsoleColor.DarkRed, "Manually activated FFRK restart");
            await RestartFFRK();
        }

    }

}
