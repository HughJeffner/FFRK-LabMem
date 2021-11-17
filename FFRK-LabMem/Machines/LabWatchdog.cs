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
        private readonly Timer watchdogHangTimer = new Timer(Int32.MaxValue);
        private readonly Timer watchdogCrashTimer = new Timer(Int32.MaxValue);
        private readonly Adb adb;
        public double HangCheckInterval => watchdogHangTimer.Interval;
        public double CrashCheckInterval => watchdogCrashTimer.Interval;
        public bool Enabled { get; set; } = false;

        public event EventHandler<WatchdogEventArgs> Timeout;

        public class WatchdogEventArgs
        {
            public enum TYPE {
                Hang = 0,
                Crash = 1,
            }
            public ElapsedEventArgs ElapsedEventArgs;
            public TYPE Type;
            public override string ToString()
            {
                return Type.ToString();
            }
        }

        public LabWatchdog(Adb adb, int hangCheckMinutes, int crashCheckSeconds) : this(
            adb,
            TimeSpan.FromMinutes(hangCheckMinutes).TotalMilliseconds, 
            TimeSpan.FromSeconds(crashCheckSeconds).TotalMilliseconds
        ) { }

        public LabWatchdog(Adb adb, double hangCheckInterval, double crashCheckInterval)
        {

            this.adb = adb;

            if (hangCheckInterval > 0)
            {
                watchdogHangTimer.AutoReset = false;
                watchdogHangTimer.Interval = hangCheckInterval;
                watchdogHangTimer.Elapsed += WatchdogHangTimer_Elapsed;
            }

            if (crashCheckInterval > 0)
            {
                watchdogCrashTimer.AutoReset = true;
                watchdogCrashTimer.Interval = crashCheckInterval;
                watchdogCrashTimer.Elapsed += WatchdogCrashTimer_Elapsed;

            }

        }

        /// <summary>
        /// Kicks the watchdog
        /// </summary>
        /// <param name="restart"></param>
        public void Kick(bool restart = true)
        {
            if (!this.Enabled) return;

            watchdogHangTimer.Stop();
            watchdogCrashTimer.Stop();
            if (restart)
            {
                watchdogHangTimer.Start();
                watchdogCrashTimer.Start();
                ColorConsole.WriteLine(ConsoleColor.DarkGray, "Watchdog kicked");
            }
            else
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Watchdog paused");
            }
        }

        /// <summary>
        /// Enables the watchdog, but does not start the timers until kicked
        /// </summary>
        public void Enable(bool start = false)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Watchdog enabled");
            this.Enabled = true;
            if (start) Kick(true);
        }

        /// <summary>
        /// Stops and disables the watchdog until enabled again
        /// </summary>
        public void Disable()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Watchdog disabled");
            this.Enabled = false;
            watchdogHangTimer.Stop();
            watchdogCrashTimer.Stop();
        }

        public void Update(int hangCheckMinutes, int crashCheckSeconds)
        {
            watchdogHangTimer.Elapsed -= WatchdogHangTimer_Elapsed;
            if (hangCheckMinutes > 0)
            {
                watchdogHangTimer.Interval = TimeSpan.FromMinutes(hangCheckMinutes).TotalMilliseconds;
                watchdogHangTimer.Elapsed += WatchdogHangTimer_Elapsed;
            }
            watchdogCrashTimer.Elapsed -= WatchdogCrashTimer_Elapsed;
            if (crashCheckSeconds > 0)
            {
                watchdogCrashTimer.Interval = TimeSpan.FromSeconds(crashCheckSeconds).TotalMilliseconds;
                watchdogCrashTimer.Elapsed += WatchdogCrashTimer_Elapsed;
            }
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Updated watchdog timers; hang:{0}m, crash:{0}s", hangCheckMinutes, crashCheckSeconds);
        }

        private async void WatchdogCrashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var state = await adb.IsPackageRunning(Adb.FFRK_PACKAGE_NAME, System.Threading.CancellationToken.None);
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Watchdog ffrk state: {0}", state ? "Running" : "Not Running");
            if (!state)
            {
                InvokeTimeout(sender, WatchdogEventArgs.TYPE.Crash, e);
            }
        }

        private void WatchdogHangTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InvokeTimeout(sender, WatchdogEventArgs.TYPE.Hang, e);
        }

        private void InvokeTimeout(object sender, WatchdogEventArgs.TYPE type, ElapsedEventArgs e)
        {
            var e2 = new WatchdogEventArgs() { ElapsedEventArgs = e, Type = type };
            ColorConsole.Debug(ColorConsole.DebugCategory.Watchdog, "Watchdog fault: {0}", e2);
            Timeout?.Invoke(sender, e2);
        }
    }
}
