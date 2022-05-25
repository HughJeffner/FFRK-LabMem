using FFRK_LabMem.Data;
using FFRK_Machines;
using FFRK_Machines.Services.Adb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Config
{
    public class AppConfig
    {
        static ConfigHelper helper;
        private List<IAppConfigSection> configs;

        public class ConsoleConfig : IAppConfigSection
        {
            public bool Timestamps { get; set; } = true;
            public ColorConsole.DebugCategory DebugCategories { get; set; } = 0;
            public bool Logging { get; set; } = false;
            public String LogFolder { get; set; } = "";
            public int BufferSize { get; set; } = 10;
            public void Load(ConfigHelper helper)
            {
                Timestamps = helper.GetBool("console.timestamps", true);
                DebugCategories = (ColorConsole.DebugCategory)helper.GetInt("console.debugCategories", 0);
                Logging = helper.GetBool("console.logging", false);
                LogFolder = helper.GetString("console.logFolder", "");
                BufferSize = helper.GetInt("console.logBuffer", 10);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    { "console.timestamps", Timestamps },
                    { "console.debugCategories", DebugCategories },
                    { "console.logging", Logging },
                    { "console.logFolder", LogFolder },
                    { "console.logBuffer", BufferSize }
                });
            }
        }

        public class UpdateConfig : IAppConfigSection
        {
            public bool CheckForUpdates { get; set; } = false;
            public bool IncludePrerelease { get; set; } = false;
            public void Load(ConfigHelper helper)
            {
                CheckForUpdates = helper.GetBool("updates.checkForUpdates", false);
                IncludePrerelease = helper.GetBool("updates.includePrerelease", false);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"updates.checkForUpdates", CheckForUpdates },
                    {"updates.includePrerelease", IncludePrerelease }
                });
            }
        }

        public class ScreenConfig : IAppConfigSection
        {
            public int TopOffset { get; set; } = -1;
            public int BottomOffset { get; set; } = -1;
            public void Load(ConfigHelper helper)
            {
                TopOffset = helper.GetInt("screen.topOffset", -1);
                BottomOffset = helper.GetInt("screen.bottomOffset", -1);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"screen.topOffset", TopOffset },
                    {"screen.bottomOffset", BottomOffset }
                });
            }
        }

        public class AdbConfig : IAppConfigSection
        {
            public bool CloseOnExit { get; set; } = false;
            public string Path { get; set; } = "adb.exe";
            public string Host { get; set; } = "127.0.0.1:7555";
            public Adb.CaptureType Capture { get; set; } = FFRK_Machines.Services.Adb.Adb.CaptureType.Minicap;
            public int CaptureRate { get; set; } = 200;
            public double FindPrecision { get; set; } = 0.5;
            public int FindAccuracy { get; set; } = 0;
            public Adb.InputType Input { get; set; } = FFRK_Machines.Services.Adb.Adb.InputType.Minitouch;
            public int TapDelay { get; set; } = 30;
            public int TapDuration { get; set; } = 0;
            public int TapPressure { get; set; } = 50;
            public String ScreenshotFolder { get; set; } = "";
            public void Load(ConfigHelper helper)
            {
                CloseOnExit = helper.GetBool("adb.closeOnExit", false);
                Path = helper.GetString("adb.path", "adb.exe");
                Host = helper.GetString("adb.host", "127.0.0.1:7555");
                Capture = helper.GetEnum("adb.capture", FFRK_Machines.Services.Adb.Adb.CaptureType.Minicap);
                CaptureRate = helper.GetInt("adb.captureRate", 200);
                FindPrecision = helper.GetDouble("adb.findPrecision", 0.5);
                FindAccuracy = helper.GetInt("adb.findAccuracy", 0);
                Input = helper.GetEnum("adb.input", FFRK_Machines.Services.Adb.Adb.InputType.Minitouch);
                TapDelay = helper.GetInt("adb.tapDelay", 30);
                TapDuration = helper.GetInt("adb.tapDuration", 0);
                TapPressure = helper.GetInt("adb.tapPressure", 50);
                ScreenshotFolder = helper.GetString("adb.screenshotFolder", "");
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"adb.closeOnExit", CloseOnExit },
                    {"adb.path", Path },
                    {"adb.host", Host },
                    {"adb.capture", (int)Capture },
                    {"adb.captureRate", CaptureRate },
                    {"adb.findPrecision", FindPrecision },
                    {"adb.findAccuracy", FindAccuracy },
                    {"adb.input", (int)Input },
                    {"adb.tapDelay", TapDelay },
                    {"adb.tapDuration", TapDuration },
                    {"adb.tapPressure", TapPressure },
                    {"adb.screenshotFolder", ScreenshotFolder },
                });
            }
        }

        public class DataLogConfig : IAppConfigSection
        {
            public bool Enabled { get; set; } = false;
            public void Load(ConfigHelper helper)
            {
                Enabled = helper.GetBool("datalogger.enabled", false);
            }
            public void Save()
            {
                helper.SetValue("datalogger.enabled", Enabled);
            }
        }

        public class CountersConfig : IAppConfigSection
        {
            public Counters.DropCategory DropCategories { get; set; } = (Counters.DropCategory)15;
            public bool LogDropsToTotalCounters { get; set; } = false;
            public int MaterialsRarityFilter { get; set; } = 6;
            public void Load(ConfigHelper helper)
            {
                DropCategories = (Counters.DropCategory)helper.GetInt("counters.dropCategories", 15);
                LogDropsToTotalCounters = helper.GetBool("counters.logDropsToTotal", false);
                MaterialsRarityFilter = helper.GetInt("counters.materialsRarityFilter", 6);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"counters.dropCategories", (int)DropCategories },
                    {"counters.logDropsToTotal", LogDropsToTotalCounters },
                    {"counters.materialsRarityFilter", MaterialsRarityFilter }
                });
            }
        }

        public class LabConfig : IAppConfigSection
        {
            public string ConfigFile { get; set; } = "config/lab.balanced.json";
            public int WatchdogHangSeconds { get; set; } = 120;
            public int WatchdogHangWarningSeconds { get; set; } = 60;
            public int WatchdogHangWarningLoopThreshold { get; set; } = 10;
            public bool WatchdogHangScreenshot { get; set; } = false;
            public int WatchdogBattleMinutes { get; set; } = 15;
            public int WatchdogCrashSeconds { get; set; } = 30;
            public int WatchdogMaxRetries { get; set; } = 5;
            public int WatchdogRestartLoopThreshold { get; set; } = 6;
            public int WatchdogRestartLoopWindowMinutes { get; set; } = 60;
            public int WatchdogBattleMaxRetries { get; set; } = 5;
            public void Load(ConfigHelper helper)
            {
                ConfigFile = helper.GetString("lab.configFile", "config/lab.balanced.json");
                WatchdogHangSeconds = helper.GetInt("lab.watchdogHangSeconds", 120);
                WatchdogHangWarningSeconds = helper.GetInt("lab.watchdogHangWarningSeconds", 60);
                WatchdogHangWarningLoopThreshold = helper.GetInt("lab.watchdogHangWarningLoopThreshold", 10);
                WatchdogHangScreenshot = helper.GetBool("lab.watchdogHangScreenshot", false);
                WatchdogBattleMinutes = helper.GetInt("lab.watchdogBattleMinutes", 15);
                WatchdogCrashSeconds = helper.GetInt("lab.watchdogCrashSeconds", 30);
                WatchdogMaxRetries = helper.GetInt("lab.watchdogMaxRetries", 5);
                WatchdogRestartLoopThreshold = helper.GetInt("lab.watchdogLoopDetectionThreshold", 6);
                WatchdogRestartLoopWindowMinutes = helper.GetInt("lab.watchdogLoopDetectionWindowMinutes", 60);
                WatchdogBattleMaxRetries = helper.GetInt("lab.watchdogBattleMaxRetries", 5);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"lab.configFile", ConfigFile },
                    {"lab.watchdogHangSeconds", WatchdogHangSeconds },
                    {"lab.watchdogHangWarningSeconds", WatchdogHangWarningSeconds },
                    {"lab.watchdogHangWarningLoopThreshold", WatchdogHangWarningLoopThreshold },
                    {"lab.watchdogHangScreenshot", WatchdogHangScreenshot },
                    {"lab.watchdogBattleMinutes", WatchdogBattleMinutes },
                    {"lab.watchdogCrashSeconds", WatchdogCrashSeconds },
                    {"lab.watchdogMaxRetries", WatchdogMaxRetries },
                    {"lab.watchdogLoopDetectionThreshold", WatchdogRestartLoopThreshold },
                    {"lab.watchdogLoopDetectionWindowMinutes", WatchdogRestartLoopWindowMinutes },
                    {"lab.watchdogBattleMaxRetries", WatchdogBattleMaxRetries },
                });
            }
        }

        public class ProxyConfig : IAppConfigSection
        {
            public int Port { get; set; } = 8081;
            public bool Secure { get; set; } = true;
            public string Blocklist { get; set; } = "";
            public bool ConnectionPooling { get; set; } = false;
            public bool AutoConfig { get; set; } = false;
            public void Load(ConfigHelper helper)
            {
                Port = helper.GetInt("proxy.port", 8081);
                Secure = helper.GetBool("proxy.secure", false);
                Blocklist = helper.GetString("proxy.blocklist", "");
                ConnectionPooling = helper.GetBool("proxy.connectionPooling", false);
                AutoConfig = helper.GetBool("proxy.autoconfig", false);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"proxy.port", Port },
                    {"proxy.secure", Secure },
                    {"proxy.blocklist", Blocklist },
                    {"proxy.connectionPooling", ConnectionPooling },
                    {"proxy.autoconfig", AutoConfig }
                });
            }
        }

        public class SchedulerConfig : IAppConfigSection
        {
            public int MaintenanceDoneHourUTC { get; set; } = 13;
            public void Load(ConfigHelper helper)
            {
                MaintenanceDoneHourUTC = helper.GetInt("scheduler.maintenanceDoneHourUtc", 13);
            }
            public void Save()
            {
                helper.SetValues(new Dictionary<string, IConvertible>()
                {
                    {"scheduler.maintenanceDoneHourUtc", MaintenanceDoneHourUTC }
                });
            }
        }

        public ConsoleConfig Console { get; private set; } = new ConsoleConfig();
        public UpdateConfig Updates { get; private set; } = new UpdateConfig();
        public ScreenConfig Screen { get; private set; } = new ScreenConfig();
        public AdbConfig Adb { get; private set; } = new AdbConfig();
        public DataLogConfig DataLog { get; private set; } = new DataLogConfig();
        public CountersConfig Counters { get; private set; } = new CountersConfig();
        public LabConfig Lab { get; private set; } = new LabConfig();
        public ProxyConfig Proxy { get; private set; } = new ProxyConfig();
        public SchedulerConfig Scheduler { get; private set; } = new SchedulerConfig();

        public AppConfig(String configFile)
        {

            configs = new List<IAppConfigSection>()
            {
                Console,
                Updates,
                Screen,
                Adb,
                DataLog,
                Counters,
                Lab,
                Proxy,
                Scheduler
            };

            try
            {
                helper = new ConfigHelper(configFile);
                foreach (var item in configs)
                {
                    item.Load(helper);
                }
            } catch (Exception e)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, $"Error loading config file, using default values: {e}");
            }
        }

        public void SaveAll()
        {
            foreach (var item in configs)
            {
                item.Save();
            }
        }

    }
}
