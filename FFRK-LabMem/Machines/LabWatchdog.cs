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
            public int HangSeconds { get; set; } = 120;
            public int HangWarningSeconds { get; set; } = 60;
            public bool HangScreenshot { get; set; } = false;
            public int BattleMinutes { get; set; } = 15;
            public int CrashSeconds { get; set; } = 30;
            public int MaxRetries { get; set; } = 5;
            public int RestartLoopThreshold { get; set; } = 6;
            public int RestartLoopWindowMinutes { get; set; } = 60;
            public int BattleMaxRetries { get; set; } = 5;
        }

        private readonly Timer watchdogHangTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogHangWarningTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogBattleTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogCrashTimer = new Timer(Int32.MaxValue);
        private List<DateTime> pastRestarts = new List<DateTime>();
        private int battleTries = 0;
        private Lab Lab { get; set; }
        public bool Enabled { get; set; } = false;
        public Configuration Config { get; private set; } = new Configuration();
        public event EventHandler<WatchdogEventArgs> Timeout;
        public event EventHandler<WatchdogEventArgs> Warning;
        public event EventHandler<WatchdogEventArgs> RestartLoop;
        public event EventHandler<WatchdogEventArgs> BattleLoop;

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
            watchdogHangWarningTimer.AutoReset = false;
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

            watchdogHangTimer.Stop();
            watchdogHangWarningTimer.Stop();
            watchdogBattleTimer.Stop();
            watchdogCrashTimer.Stop();
            if (restart)
            {
                watchdogHangTimer.Start();
                watchdogHangWarningTimer.Start();
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
        /// Signals a battle failure and checks the retry count
        /// </summary>
        /// <returns>true if the retry count is still under the threshold, false otherwise</returns>
        public bool BattleFailed()
        {
            if (Config.BattleMaxRetries > 0)
            {

                battleTries += 1;
                if (battleTries >= Config.BattleMaxRetries)
                {
                    battleTries = 0;
                    BattleLoop?.Invoke(this, new WatchdogEventArgs() { Type = WatchdogEventArgs.TYPE.LongBattle });
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Resets the battle failed retry counter
        /// </summary>
        public void BattleReset()
        {
            battleTries = 0;
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
            watchdogHangTimer.Stop();
            watchdogHangWarningTimer.Stop();
            watchdogBattleTimer.Stop();
            watchdogCrashTimer.Stop();
        }

        public void Update(Configuration config)
        {
            this.Config = config;
            watchdogHangTimer.Elapsed -= WatchdogHangTimer_Elapsed;
            if (config.HangSeconds > 0)
            {
                watchdogHangTimer.Interval = TimeSpan.FromSeconds(config.HangSeconds).TotalMilliseconds;
                watchdogHangTimer.Elapsed += WatchdogHangTimer_Elapsed;
            }
            watchdogHangWarningTimer.Elapsed -= WatchdogHangWarningTimer_Elapsed;
            if (config.HangWarningSeconds > 0)
            {
                watchdogHangWarningTimer.Interval = TimeSpan.FromSeconds(config.HangWarningSeconds).TotalMilliseconds;
                watchdogHangWarningTimer.Elapsed += WatchdogHangWarningTimer_Elapsed;
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
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Updated timers; hang:{0}s, battle:{1}m, crash:{2}s", config.HangSeconds, config.BattleMinutes, config.CrashSeconds);
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

        private void WatchdogHangWarningTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Ignore if in battle
            if (Lab.StateMachine.State == Lab.State.Battle) return;
   
            Warning?.Invoke(sender, new WatchdogEventArgs() { ElapsedEventArgs = e, Type = WatchdogEventArgs.TYPE.Hang });

        }

        private void WatchdogHangTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            // Ignore if in battle
            if (Lab.StateMachine.State == Lab.State.Battle)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Ignoring hang timer because in battle");
                return;
            }
           
            InvokeTimeout(sender, WatchdogEventArgs.TYPE.Hang, e);
                
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
                RestartLoop?.Invoke(sender, e2);
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
