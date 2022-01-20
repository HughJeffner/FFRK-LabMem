using FFRK_LabMem.Services;
using FFRK_Machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FFRK_LabMem.Machines
{
    public class LabWatchdog
    {

        public class Configuration
        {
            public int HangMinutes { get; set; }
            public int HangWarningRatio { get; set; } = 2;
            public int BattleMinutes { get; set; }
            public int CrashSeconds { get; set; }
            public int MaxRetries { get; set; }
            public int RestartLoopThreshold { get; set; } = 6;
            public int RestartLoopWindowMinutes { get; set; } = 60;
        }

        private readonly Timer watchdogHangTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogBattleTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogCrashTimer = new Timer(Int32.MaxValue);
        private List<DateTime> pastRestarts = new List<DateTime>();
        private int hangWarnings = 0;
        private Lab Lab { get; set; }
        public bool Enabled { get; set; } = false;
        public Configuration Config { get; private set; } = new Configuration();
        public event EventHandler<WatchdogEventArgs> Timeout;
        public event EventHandler<WatchdogEventArgs> Warning;
        public event EventHandler<WatchdogEventArgs> LoopDetected;

        public class WatchdogEventArgs
        {
            public enum TYPE {
                Hang,
                Crash,
                LongBattle
            }
            public ElapsedEventArgs ElapsedEventArgs { get; set; }
            public TYPE Type { get; set; }
            public override string ToString()
            {
                return Type.ToString();
            }
        }

        public LabWatchdog(Lab lab, Configuration config)
        {
            this.Lab = lab;
            watchdogHangTimer.AutoReset = false;
            watchdogBattleTimer.AutoReset = false;
            watchdogCrashTimer.AutoReset = true;
            Update(config);
        }

        /// <summary>
        /// Kicks the watchdog
        /// </summary>
        /// <param name="restart"></param>
        public void Kick(bool restart = true)
        {
            if (!this.Enabled) return;

            hangWarnings = 0;
            watchdogHangTimer.Stop();
            watchdogBattleTimer.Stop();
            watchdogCrashTimer.Stop();
            if (restart)
            {
                watchdogHangTimer.Start();
                watchdogBattleTimer.Start();
                watchdogCrashTimer.Start();
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Kicked");
            }
            else
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Paused");
            }
        }

        /// <summary>
        /// Enables the watchdog, but does not start the timers until kicked
        /// </summary>
        public void Enable(bool start = false)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Enabled");
            this.Enabled = true;
            if (start) Kick(true);
        }

        /// <summary>
        /// Stops and disables the watchdog until enabled again
        /// </summary>
        public void Disable()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Disabled");
            this.Enabled = false;
            hangWarnings = 0;
            watchdogHangTimer.Stop();
            watchdogBattleTimer.Stop();
            watchdogCrashTimer.Stop();
        }

        public void Update(Configuration config)
        {
            this.Config = config;
            watchdogHangTimer.Elapsed -= WatchdogHangTimer_Elapsed;
            if (config.HangMinutes > 0)
            {
                // Divide by ratio to get warning interval
                if (config.HangWarningRatio == 0) config.HangWarningRatio = 1;
                watchdogHangTimer.Interval = TimeSpan.FromMinutes(config.HangMinutes).TotalMilliseconds / config.HangWarningRatio;
                watchdogHangTimer.Elapsed += WatchdogHangTimer_Elapsed;
            }
            watchdogBattleTimer.Elapsed -= WatchdogBattleTimer_Elapsed;
            if (config.BattleMinutes > 0)
            {
                watchdogBattleTimer.Interval = TimeSpan.FromMinutes(config.BattleMinutes).TotalMilliseconds;
                watchdogBattleTimer.Elapsed += WatchdogBattleTimer_Elapsed;
            }
            watchdogCrashTimer.Elapsed -= WatchdogCrashTimer_Elapsed;
            if (config.CrashSeconds > 0)
            {
                watchdogCrashTimer.Interval = TimeSpan.FromSeconds(config.CrashSeconds).TotalMilliseconds;
                watchdogCrashTimer.Elapsed += WatchdogCrashTimer_Elapsed;
            }
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Updated timers; hang:{0}m, battle:{1}m, crash:{2}s", config.HangMinutes, config.BattleMinutes, config.CrashSeconds);
        }

        private async void WatchdogCrashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var state = await Lab.Adb.IsPackageRunning(Adb.FFRK_PACKAGE_NAME, System.Threading.CancellationToken.None);
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "FFRK state: {0}", state ? "Running" : "Not Running");
            if (!state)
            {
                InvokeTimeout(sender, WatchdogEventArgs.TYPE.Crash, e);
            }
        }

        private void WatchdogHangTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            // Ignore if in battle
            if (Lab.StateMachine.State == Lab.State.Battle)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Ignoring hang timer because in battle");
                return;
            }

            // Warning checks
            hangWarnings += 1;
            if (hangWarnings < Config.HangWarningRatio)
            {
                // Only on first
                if (hangWarnings == 1) Warning?.Invoke(sender, new WatchdogEventArgs() { ElapsedEventArgs = e, Type = WatchdogEventArgs.TYPE.Hang });
                watchdogHangTimer.Start();
            } else
            {
                InvokeTimeout(sender, WatchdogEventArgs.TYPE.Hang, e);
                hangWarnings = 0;
            }
                
        }

        private void WatchdogBattleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            // Ignore if not in battle
            if (Lab.StateMachine.State != Lab.State.Battle)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Ignoring battle timer because not in battle");
                return;
            }

            InvokeTimeout(sender, WatchdogEventArgs.TYPE.LongBattle, e);
        }

        private void InvokeTimeout(object sender, WatchdogEventArgs.TYPE type, ElapsedEventArgs e)
        {

            var e2 = new WatchdogEventArgs() { ElapsedEventArgs = e, Type = type };

            //Check for restart loop
            if (CheckRestartLoopWindow(e2))
            {
                LoopDetected?.Invoke(sender, e2);
                return;
            }
                        
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Fault: {0}", e2);
            Timeout?.Invoke(sender, e2);
        }

        private bool CheckRestartLoopWindow(WatchdogEventArgs args)
        {

            // Ignore crash detection events
            if (args.Type == WatchdogEventArgs.TYPE.Crash) return false;

            // Current instant in time
            var cmp = DateTime.Now;

            // Remove all expired events
            pastRestarts.RemoveAll(i => (cmp - i).TotalMinutes >= Config.RestartLoopWindowMinutes);

            // Add the newest to the end of the list
            pastRestarts.Add(cmp);

            // If we reached or exceeded the threshold count
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, $"Number of restarts in loop detection window: {pastRestarts.Count}/{Config.RestartLoopThreshold}");
            if (pastRestarts.Count > Config.RestartLoopThreshold)
            {
                // Restart loop detected
                return true;
            }

            return false;

        }
    }
}
