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

        private ConfigHelper configHelper;
        private bool isQuickExploring = false;
        public static async Task<LabController> Create(ConfigHelper config)
        {

            // Create instance
            var ret = new LabController
            {
                configHelper = config
            };

            // Validate config file
            var configFilePath = config.GetString("lab.configFile", "Config/lab.balanced.json");
            if (!File.Exists(configFilePath))
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not load {0}!", configFilePath);
                return ret;
            }
            var findPrecision = config.GetDouble("adb.findPrecision", 0.5);
            if (findPrecision < 0 || findPrecision > 1) config.SetValue("adb.findPrecision", 0.5);

            // Services
            await DataLogger.Initalize(config);
            await Counters.Initalize(config, ret);
            await Notifications.Initalize();

            return ret;

        }

        public static async Task<LabController> CreateAndStart(ConfigHelper config)
        {

            var ret = await Create(config);

            // Start it
            var args = new StartArguments()
            {
                AdbPath = config.GetString("adb.path", "adb.exe"),
                AdbHost = config.GetString("adb.host", "127.0.0.1:7555"),
                ProxyPort = config.GetInt("proxy.port", 8081),
                ProxySecure = config.GetBool("proxy.secure", false),
                ProxyBlocklist = config.GetString("proxy.blocklist", ""),
                ProxyConnectionPooling = config.GetBool("proxy.connectionPooling", false),
                ProxyAutoConfig = config.GetBool("proxy.autoconfig", false),
                ConfigFile = config.GetString("lab.configFile", "Config/lab.balanced.json"),
                TopOffset = config.GetInt("screen.topOffset", -1),
                BottomOffset = config.GetInt("screen.bottomOffset", -1),
                Capture = config.GetEnum("adb.capture", Adb.CaptureType.Minicap),
                CaptureRate = config.GetInt("adb.captureRate", 200),
                FindPrecision = config.GetDouble("adb.findPrecision", 0.5),
                FindAccuracy = config.GetInt("adb.findAccuracy", 0),
                Input = config.GetEnum("adb.input", Adb.InputType.Minitouch),
                TapDelay = config.GetInt("adb.tapDelay", 30),
                TapDuration = config.GetInt("adb.tapDuration", 0),
                TapPressure = config.GetInt("adb.tapPressure", 50),
                Consumers = 2,
                ScreenshotFolder = config.GetString("adb.screenshotFolder", "")
            };
            await ret.Start(args);

            // Auto-detect offsets
            if (ret.Adb != null && ret.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets not set up, press [Alt+O] to detect them once FFRK is on the Home screen");
            }

            // Scheduler
            await Services.Scheduler.Init(ret);
            Services.Scheduler.Default.MaintenanceDoneHourUtc = config.GetInt("scheduler.maintenanceDoneHourUtc", 13);

            return ret;
        }
        protected override Lab CreateMachine(LabConfiguration config)
        {
            var watchdogConfig = new LabWatchdog.Configuration()
            {
                HangSeconds = configHelper.GetInt("lab.watchdogHangSeconds", 120),
                HangWarningSeconds = configHelper.GetInt("lab.watchdogHangWarningSeconds", 60),
                HangWarningLoopThreshold = configHelper.GetInt("lab.watchdogHangWarningLoopThreshold", 10),
                HangScreenshot = configHelper.GetBool("lab.watchdogHangScreenshot", false),
                BattleMinutes = configHelper.GetInt("lab.watchdogBattleMinutes", 15),
                CrashSeconds = configHelper.GetInt("lab.watchdogCrashSeconds", 30),
                MaxRetries = configHelper.GetInt("lab.watchdogMaxRetries", 5),
                RestartLoopThreshold = configHelper.GetInt("lab.watchdogLoopDetectionThreshold", 6),
                RestartLoopWindowMinutes = configHelper.GetInt("lab.watchdogLoopDetectionWindowMinutes", 60),
                BattleMaxRetries = configHelper.GetInt("lab.watchdogBattleMaxRetries", 5)
            };
            return new Lab(this.Adb, config, watchdogConfig);
        }

        public async void AutoDetectOffsets(ConfigHelper config) {

            if (this.Adb != null && this.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detecting screen offsets...");
                var offsets = await this.Adb.GetOffsets("#151515", 2000, System.Threading.CancellationToken.None);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detected offset t:{0}, b:{1}, saving to .config", offsets.Item1, offsets.Item2);
                this.Adb.TopOffset = offsets.Item1;
                this.Adb.BottomOffset = offsets.Item2;
                config.SetValue("screen.topOffset", offsets.Item1);
                config.SetValue("screen.bottomOffset", offsets.Item2);
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
