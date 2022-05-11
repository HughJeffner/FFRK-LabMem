using System;
using System.Linq;
using System.Threading.Tasks;
using FFRK_Machines.Services;
using Newtonsoft.Json.Linq;
using Stateless;
using System.Diagnostics;
using FFRK_Machines.Machines;
using FFRK_Machines;
using FFRK_LabMem.Data;
using System.Collections.Generic;
using FFRK_Machines.Threading;
using FFRK_Machines.Services.Notifications;
using FFRK_Machines.Services.Adb;

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
            MoveOn,
            StartBattle,
            EnterDungeon,
            BattleSuccess,
            BattleFailed,
            FoundBoss,
            MissedButton,
            FinishedLab,
            Restart,
            EnteredOutpost
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
            Restarting,
            Outpost
        }

        public int CurrentKeys { get; set; }
        public int CurrentTears { get; set; }
        public JToken CurrentPainting { get; set; }
        public int CurrentFloor { get; set; }
        public int FinalFloor { get; set; }
        public int SelectedPartyIndex { get; set; } = 0;
        public int RestartLabCounter { get; set; } = -1;
        public LabWatchdog Watchdog { get; }
        public LabFatigueInfo FatigueInfo { get; set; } = new LabFatigueInfo();
        public LabStaminaInfo StaminaInfo { get; set; } = new LabStaminaInfo();

        public readonly AsyncAutoResetEvent AutoResetEventQuickExplore = new AsyncAutoResetEvent(false);
        private bool disableSafeRequested = false;
        private readonly Stopwatch battleStopwatch = new Stopwatch();
        private readonly Stopwatch recoverStopwatch = new Stopwatch();
        private int restartTries = 0;
        private LabParser parser;
        private LabSelector selector;

        public Lab(Adb adb, LabConfiguration config, LabWatchdog.Configuration watchdogConfig)
        {

            // Config
            this.Config = config;
            this.Adb = adb;
            this.Watchdog = new LabWatchdog(this, watchdogConfig);
            this.Watchdog.Timeout += Watchdog_Timeout;
            this.Watchdog.Warning += Watchdog_Warning;
            this.Watchdog.RestartLoop += Watchdog_RestartLoop;
            this.Watchdog.BattleLoop += Watchdog_BattleLoop;
            this.parser = new LabParser(this);
            this.selector = new LabSelector(this);
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
                .Permit(Trigger.FinishedLab, State.Completed)
                .IgnoreIf(Trigger.EnteredOutpost, () => !recoverStopwatch.IsRunning)
                .PermitIf(Trigger.EnteredOutpost, State.Outpost, () => recoverStopwatch.IsRunning);

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
                .Permit(Trigger.FinishedLab, State.Completed)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.FoundThing)
                .OnEntryFromAsync(Trigger.FoundThing, async (t) => await MoveOn(false))
                .OnEntryFromAsync(Trigger.FoundPortal, async(t) => await MoveOn(true))
                .Permit(Trigger.MoveOn, State.Ready)
                .Permit(Trigger.MissedButton, State.Unknown);

            this.StateMachine.Configure(State.FoundTreasure)
                .OnEntryAsync(async (t) => await SelectTreasures(t))
                .PermitReentry(Trigger.FoundTreasure)
                .Permit(Trigger.MoveOn, State.Ready);

            this.StateMachine.Configure(State.FoundSealedDoor)
                .OnEntryAsync(async (t) => await OpenSealedDoor())
                .PermitReentry(Trigger.FoundDoor)
                .Permit(Trigger.FoundBattle, State.EquipParty)
                .Permit(Trigger.FoundThing, State.FoundThing)
                .Permit(Trigger.FoundTreasure, State.FoundTreasure);

            this.StateMachine.Configure(State.BattleInfo)
                .OnEntryAsync(async (t) => await EnterDungeon())
                .Permit(Trigger.EnterDungeon, State.EquipParty)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.ResetState, State.Ready)
                .Ignore(Trigger.MissedButton);

            this.StateMachine.Configure(State.EquipParty)
                .OnEntryAsync(async (t) => await StartBattle())
                .PermitReentry(Trigger.FoundBattle)
                .Permit(Trigger.StartBattle, State.Battle)
                .Permit(Trigger.ResetState, State.Ready)
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
                .Permit(Trigger.ResetState, State.Unknown)
                .Ignore(Trigger.EnteredOutpost)
                .Ignore(Trigger.PickedCombatant);

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

            this.StateMachine.Configure(State.Outpost)
                .OnEntryAsync(async (t) => await EnterOutpost())
                .Permit(Trigger.FinishedLab, State.Completed);

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
            // Message only if not from self
            if (sender != this) ColorConsole.WriteLine(ConsoleColor.DarkRed, "{0} detected!", e.Type);

            // Auto-battle check for wait mode
            if (e.Type == LabWatchdog.WatchdogEventArgs.TYPE.LongBattle) await CheckAutoBattle();
            
            // Counters
            if (e.Type == LabWatchdog.WatchdogEventArgs.TYPE.Crash) await Counters.FFRKCrashed();
            if (e.Type == LabWatchdog.WatchdogEventArgs.TYPE.Hang) await Counters.FFRKHang(Watchdog.Config.HangWarningSeconds > 0);

            // On a timer thread, need to handle errors
            try
            {
                //Restart ffrk and get result, restart watchdog if failed
                if (!await RestartFFRK())
                {

                    // Limit number of retries
                    if (restartTries < Watchdog.Config.MaxRetries)
                    {
                        restartTries += 1;
                        ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Starting watchdog after failed FFRK restart (try {0})", restartTries);
                        Watchdog.Kick();
                    }
                    else
                    {
                        // Retries exhausted
                        await Notify(Notifications.EventType.LAB_FAULT, "Max FFRK restarts reached");
                        OnMachineFinished();
                    }
                }
                else
                {
                    // Restart success, reset tries
                    restartTries = 0;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
            }

        }
        private async void Watchdog_Warning(object sender, LabWatchdog.WatchdogEventArgs e)
        {

            // List of valid states for each action
            List<State> autoStartStates = new List<State>() {
                State.Unknown,
                State.FoundSealedDoor,
                State.FoundThing,
                State.FoundTreasure,
                State.Ready,
                State.PortalConfirm,
                State.EquipParty,
                State.BattleInfo,
                State.BattleFinished
            };

            List<State> backStates = new List<State>
            {
                State.Ready,
                State.BattleInfo,
                State.EquipParty
            };

            // On a timer thread, need to handle errors
            try
            {
                // Message and screenshot
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Possible hang, attempting recovery!");
                if (Watchdog.Config.HangScreenshot) await Adb.SaveScreenshot(String.Format("hang_{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss")), this.CancellationToken);

                // Keep track of current state in case it changes
                var previousState = StateMachine.State;

                // Navigate back
                if (backStates.Contains(StateMachine.State))
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkGray, "Navigating back");
                    for (int i = 0; i < 3; i++)
                    {
                        if (StateMachine.State == State.Battle) break;
                        await Adb.NavigateBack(this.CancellationToken);
                        await Task.Delay(500);
                    }
                    await Task.Delay(3000);
                }

                // Auto start if no state transition
                if (autoStartStates.Contains(StateMachine.State) && StateMachine.State.Equals(previousState))
                {
                    await AutoStart();
                }
                await Counters.FFRKRecovered();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
            }
           
        }
        private async void Watchdog_RestartLoop(object sender, LabWatchdog.WatchdogEventArgs e)
        {
            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Restart loop detected!");
            await Notify(Notifications.EventType.LAB_FAULT, "Restart loop detected");
            OnMachineFinished();
        }

        private async void Watchdog_BattleLoop(object sender, LabWatchdog.WatchdogEventArgs e)
        {
            ColorConsole.WriteLine(ConsoleColor.DarkRed, "Battle loop detected!");
            await Notify(Notifications.EventType.LAB_FAULT, "Battle loop detected");
            OnMachineFinished();
        }

        public override void RegisterWithProxy(Proxy Proxy)
        {
            Proxy.AddRegistration("get_display_paintings", parser.ParseDisplayPaintings);
            Proxy.AddRegistration("select_painting", async (args) =>
            {
                await Adb.StopTaps();
                await parser.ParsePainting(args);
            });
            Proxy.AddRegistration("choose_explore_painting", parser.ParsePainting);
            Proxy.AddRegistration("open_treasure_chest", async (args) =>
            {
                this.Data = args.Data;
                await this.StateMachine.FireAsync(Trigger.FoundTreasure);
            });
            Proxy.AddRegistration("dungeon_recommend_info", async(args) => 
            {
                await Adb.StopTaps();
                if (this.Data != null) await this.StateMachine.FireAsync(Trigger.PickedCombatant); 
            });
            Proxy.AddRegistration("labyrinth/[0-9]+/win_battle", async(args) => 
            {
                this.Data = args.Data;
                // Prevent unexpected state change if error present
                if (args.Data["error"] == null) await this.StateMachine.FireAsync(Trigger.BattleSuccess);
            });
            Proxy.AddRegistration("continue/get_info", async(args) =>
            {
                await this.StateMachine.FireAsync(Trigger.BattleFailed);
            });
            Proxy.AddRegistration("labyrinth/[0-9]+/get_battle_init_data", async(args) => {
                recoverStopwatch.Stop();
                Watchdog.HangReset(); // Started battle indicates we not in hang state
                await this.StateMachine.FireAsync(Trigger.StartBattle);
            });
            Proxy.AddRegistration("labyrinth/party/list", parser.ParsePartyList);
            Proxy.AddRegistration("labyrinth/buddy/info", parser.ParseFatigueInfo);
            Proxy.AddRegistration(@"/dff/\?timestamp=[0-9]+", async(args) => {
                if (!await parser.ParseAllData(args))
                {
                    if (args.Body.IndexOf("maintenance", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Data is null during maintenance
                        ColorConsole.WriteLine(ConsoleColor.Red, "Maintenance ongoing, disabling...");
                        await Services.Scheduler.Default.AddPostMaintenanceSchedule();
                        OnMachineFinished();
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, "System error...");
                    }
                }
            });
            Proxy.AddRegistration("labyrinth/[0-9]+/do_simple_explore", parser.ParseQEData);
            Proxy.AddRegistration("labyrinth/[0-9]+/enter_labyrinth_dungeon", parser.ParseEnterLab);
            Proxy.AddRegistration("labyrinth/[0-9]+/get_data", async (args) =>
            {
                await this.StateMachine.FireAsync(Trigger.EnteredOutpost);
            });
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
            battleStopwatch.Reset();

            // Reset status
            this.Data = null;
            this.CurrentPainting = null;
            this.CurrentFloor = 0;
            this.FinalFloor = 0;
            this.CurrentKeys = 0;
            this.CurrentTears = 0;
            this.SelectedPartyIndex = 0;
            this.RestartLabCounter = -1;
            this.FatigueInfo.Clear();
            this.StaminaInfo.Clear();
            AutoResetEventQuickExplore.Reset();
            restartTries = 0;
            disableSafeRequested = false;
            Watchdog.BattleReset();

        }

        protected override async void OnDataChanged(JObject data)
        {

            await parser.ParseDataChanged(data);

        }

        protected async override void OnMachineError(Exception e)
        {
            if (e is InvalidStateException<Trigger,State>)
            {
                // Notification
                ColorConsole.WriteLine(ConsoleColor.Red, e.Message);
                await Notify(Notifications.EventType.LAB_FAULT, "Unexpected state");
                
                // Handle invalid states by brute-force reset of FFRK
                await ManualFFRKRestart(false);

            }
            else
            {
                base.OnMachineError(e);
            }
            
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
            await Task.Run(()=>
            {
                Watchdog_Timeout(this, new LabWatchdog.WatchdogEventArgs() { Type = LabWatchdog.WatchdogEventArgs.TYPE.Manual });
            });
            
        }

    }

}
