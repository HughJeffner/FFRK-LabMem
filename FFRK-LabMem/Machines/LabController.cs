using System;
using System.IO;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
using FFRK_LabMem.Data;
using FFRK_Machines;
using FFRK_Machines.Services.Adb;
using FFRK_Machines.Machines;
using FFRK_Machines.Services.Notifications;

namespace FFRK_LabMem.Machines
{
    public class LabController : MachineController<Lab, Lab.State, Lab.Trigger, LabConfiguration>
    {

        private AppConfig appConfig;
        private bool isQuickExploring = false;
        public static async Task<LabController> Create(AppConfig config)
        {

            // Create instance
            var ret = new LabController
            {
                appConfig = config
            };

            // Validate config file
            if (!File.Exists(config.Lab.ConfigFile))
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not load {0}!", config.Lab.ConfigFile);
                return ret;
            }
            if (config.Adb.FindPrecision < 0 || config.Adb.FindPrecision > 1) config.Adb.FindPrecision = 0.5;

            // Services
            await DataLogger.Initalize(config);
            await Counters.Initalize(config, ret);
            await Notifications.Initalize();

            return ret;

        }

        public static async Task<LabController> CreateAndStart(AppConfig config)
        {

            var ret = await Create(config);

            // Start it
            var args = new StartArguments()
            {
                AdbPath = config.Adb.Path,
                AdbHost = config.Adb.Host,
                ProxyPort = config.Proxy.Port,
                ProxySecure = config.Proxy.Secure,
                ProxyBlocklist = config.Proxy.Blocklist,
                ProxyConnectionPooling = config.Proxy.ConnectionPooling,
                ProxyAutoConfig = config.Proxy.AutoConfig,
                ConfigFile = config.Lab.ConfigFile,
                TopOffset = config.Screen.TopOffset,
                BottomOffset = config.Screen.BottomOffset,
                Capture = config.Adb.Capture,
                CaptureRate = config.Adb.CaptureRate,
                FindPrecision = config.Adb.FindPrecision,
                FindAccuracy = config.Adb.FindAccuracy,
                Input = config.Adb.Input,
                TapDelay = config.Adb.TapDelay,
                TapDuration = config.Adb.TapDuration,
                TapPressure = config.Adb.TapPressure,
                Consumers = 2,
                ScreenshotFolder = config.Adb.ScreenshotFolder
            };
            await ret.Start(args);

            // Auto-detect offsets
            if (ret.Adb != null && ret.Adb.HasDevice && config.Screen.TopOffset == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets not set up, press [Alt+O] to detect them once FFRK is on the Home screen");
            }

            // Scheduler
            await Services.Scheduler.Init(ret);
            Services.Scheduler.Default.MaintenanceDoneHourUtc = config.Scheduler.MaintenanceDoneHourUTC;

            return ret;
        }
        protected override Lab CreateMachine(LabConfiguration config)
        {
            var watchdogConfig = new LabWatchdog.Configuration()
            {
                HangSeconds = appConfig.Lab.WatchdogHangSeconds,
                HangWarningSeconds = appConfig.Lab.WatchdogHangWarningSeconds,
                HangWarningLoopThreshold = appConfig.Lab.WatchdogHangWarningLoopThreshold,
                HangScreenshot = appConfig.Lab.WatchdogHangScreenshot,
                BattleMinutes = appConfig.Lab.WatchdogBattleMinutes,
                CrashSeconds = appConfig.Lab.WatchdogCrashSeconds,
                MaxRetries = appConfig.Lab.WatchdogMaxRetries,
                RestartLoopThreshold = appConfig.Lab.WatchdogRestartLoopThreshold,
                RestartLoopWindowMinutes = appConfig.Lab.WatchdogRestartLoopWindowMinutes,
                BattleMaxRetries = appConfig.Lab.WatchdogBattleMaxRetries
            };
            return new Lab(this.Adb, config, watchdogConfig);
        }

        public async void AutoDetectOffsets(AppConfig config) {

            if (this.Adb != null && this.Adb.HasDevice && config.Screen.TopOffset == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detecting screen offsets...");
                var offsets = await this.Adb.GetOffsets("#151515", 2000, System.Threading.CancellationToken.None);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detected offset t:{0}, b:{1}, saving to .config", offsets.Item1, offsets.Item2);
                this.Adb.TopOffset = offsets.Item1;
                this.Adb.BottomOffset = offsets.Item2;
                config.Screen.TopOffset = offsets.Item1;
                config.Screen.BottomOffset = offsets.Item2;
                config.Save();
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets already set ({0},{1}).  Please set them in the .config file to '-1' for auto-detect", this.Adb.TopOffset, this.Adb.BottomOffset);
            }

        }

        public async void ManualFFRKRestart()
        {

            if (Enabled) await this.Machine.ManualFFRKRestart();

        }

        public void QuickExplore()
        {

            if (Enabled)
            {

                if (isQuickExploring)
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Quick Explore already in progress");
                    return;
                }
                isQuickExploring = true;

                int times = 0;
                int max = 0;

                // Prompt for times
                if (this.Machine.Config.RestartLab)
                {
                    ColorConsole.Write("Number of times to QE? [0-999] (0 for unlimited): 0");
                    Console.CursorLeft -= 1;
                    max = ColorConsole.ReadNumber(10);

                } else
                {
                    max = 1;
                }

                // Check for cancel
                if (max < 0)
                {
                    ColorConsole.WriteLine("Quick Explore cancelled");
                    isQuickExploring = false;
                    return;
                }

                // Pause the watchdog
                Machine.Watchdog.Kick(false);

                // Run as new task
                Task.Run(async () =>
                {

                    ColorConsole.WriteLine("Starting Quick Explore ('D' to Disable)");
                    while (this.Enabled && (max==0 || times < max))
                    {
                        try
                        {
                            ColorConsole.WriteLine("Quick explore {0} of {1}", times + 1, max == 0 ? "Unlimited" : max.ToString());
                            if (!await this.Machine.QuickExplore()) break;
                            times += 1;
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                        }
                        if (!this.Machine.Config.RestartLab)
                        {
                            ColorConsole.WriteLine("Stopping because Restart Lab control not enabled");
                            break;
                        }
                    }
                    ColorConsole.WriteLine($"Quick explore(s) complete, {times} total");
                    isQuickExploring = false;
                });
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Bot disabled, enable with 'E' first");
            }

        }

        public void SetRestartCount(int count)
        {
            if (Enabled)
            {
                if (Machine.Config.RestartLab)
                {
                    if (count > 0)
                    {
                        ColorConsole.WriteLine("Setting lab restart count limit to {0}", count);
                    } else if (count == 0)
                    {
                        ColorConsole.WriteLine("Disabling lab after this run", count);
                    } else
                    {
                        ColorConsole.WriteLine("Setting lab restart count limit to unlimited", count);
                    }
                    Machine.RestartLabCounter = count;
                } else
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Ignoring because Restart Lab control not enabled");
                }
                
            }
            
        }

    }
}
