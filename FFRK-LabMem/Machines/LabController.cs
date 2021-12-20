using System;
using System.IO;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
using FFRK_LabMem.Data;
using FFRK_Machines;
using FFRK_Machines.Machines;
using FFRK_Machines.Services.Notifications;

namespace FFRK_LabMem.Machines
{
    public class LabController : MachineController<Lab, Lab.State, Lab.Trigger, LabConfiguration>
    {

        private ConfigHelper configHelper;

        public static async Task<LabController> CreateAndStart(ConfigHelper config)
        {
            
            // Create instance
            var ret = new LabController();
            ret.configHelper = config;

            // Validate config file
            var configFilePath = config.GetString("lab.configFile", "Config/lab.balanced.json");
            if (!File.Exists(configFilePath)){
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not load {0}!", configFilePath);
                return ret;
            }

            // Services
            await DataLogger.Initalize(config);
            await Counters.Initalize(config, ret);
            await Notifications.Initalize();

            // Start it
            await ret.Start(
                adbPath: config.GetString("adb.path", "adb.exe"),
                adbHost: config.GetString("adb.host", "127.0.0.1:7555"),
                proxyPort: config.GetInt("proxy.port", 8081),
                proxySecure: config.GetBool("proxy.secure", false),
                proxyBlocklist: config.GetString("proxy.blocklist",""),
                proxyConnectionPooling: config.GetBool("proxy.connectionPooling", false),
                proxyAutoConfig: config.GetBool("proxy.autoconfig", false),
                configFile: config.GetString("lab.configFile", "Config/lab.balanced.json"),
                topOffset: config.GetInt("screen.topOffset", -1),
                bottomOffset: config.GetInt("screen.bottomOffset", -1),
                consumers: 2);
            
            // Auto-detect offsets
            if (ret.Adb != null && ret.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets not set up, press [Alt+O] to detect them once FFRK is on the Title Screen");
            }

            // Scheduler
            await Services.Scheduler.Default(ret).Start();
            
            return ret;
        }
        protected override Lab CreateMachine(LabConfiguration config)
        {
            config.WatchdogHangMinutes = configHelper.GetInt("lab.watchdogHangMinutes", 3);
            config.WatchdogBattleMinutes = configHelper.GetInt("lab.watchdogBattleMinutes", 15);
            config.WatchdogCrashSeconds = configHelper.GetInt("lab.watchdogCrashSeconds", 30);
            config.WatchdogMaxRetries = configHelper.GetInt("lab.watchdogMaxRetries", 10);
            return new Lab(this.Adb, config);
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

        public void ManualFFRKRestart()
        {

            if (Enabled)
            {

                Task.Run(async () =>
                {
                    try
                    {
                        await this.Machine.ManualFFRKRestart();
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                    }

                });
            }

        }

        public void QuickExplore()
        {

            if (Enabled)
            {

                int times = 0;
                int max = 0;

                // Prompt for times
                if (this.Machine.Config.RestartLab)
                {
                    ColorConsole.Write("Number of times to QE? [0-9] (0 for unlimited): 0");
                    Console.CursorLeft -= 1;
                    var key = ColorConsole.ReadKey(10);
                    ColorConsole.WriteLine("");
                    max = key.KeyChar - '0';
                    if (key.Key == ConsoleKey.Enter) max = 0;

                } else
                {
                    max = 1;
                }

                // Check for cancel
                if (max < 0 || max > 9)
                {
                    ColorConsole.WriteLine("Quick Explore cancelled");
                    return;
                }

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

                });
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Bot disabled, enable with 'E' first");
            }

        }

    }
}
