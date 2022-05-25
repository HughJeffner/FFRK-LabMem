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
        private ConfigHelper helper;
        public class ConsoleConfig
        {
            public bool Timestamps { get; set; } = true;
            public ColorConsole.DebugCategory DebugCategories { get; set; } = 0;
            public bool Logging { get; set; } = false;
            public String LogFolder { get; set; } = "";
            public int BufferSize { get; set; } = 10;
            public ConsoleConfig(ConfigHelper helper)
            {
                this.Timestamps = helper.GetBool("console.timestamps", true);
                this.DebugCategories = (ColorConsole.DebugCategory)helper.GetInt("console.debugCategories", 0);
                this.Logging = helper.GetBool("console.logging", false);
                this.LogFolder = helper.GetString("console.logFolder", "");
                this.BufferSize = helper.GetInt("console.logBuffer", 10);
            }
        }

        public class UpdateConfig
        {
            public bool CheckForUpdates { get; set; } = false;
            public bool IncludePrerelease { get; set; } = false;
            public UpdateConfig(ConfigHelper helper)
            {
                this.CheckForUpdates = helper.GetBool("updates.checkForUpdates", false);
                this.IncludePrerelease = helper.GetBool("updates.includePrerelease", false);
            }
        }

        public class ScreenConfig
        {
            public int TopOffset { get; set; } = -1;
            public int BottomOffset { get; set; } = -1;
            public ScreenConfig(ConfigHelper helper)
            {
                this.TopOffset = helper.GetInt("screen.topOffset", -1);
                this.BottomOffset = helper.GetInt("screen.bottomOffset", -1);
            }
            public void Save(ConfigHelper helper)
            {
                helper.SetValues(new List<KeyValuePair<string, IConvertible>>()
                {
                    new KeyValuePair<string, IConvertible>("screen.topOffset", TopOffset),
                    new KeyValuePair<string, IConvertible>("screen.bottomOffset", BottomOffset)
                });
            }
        }

        public class AdbConfig
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
            public AdbConfig(ConfigHelper helper)
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
        }

        public class DataLogConfig
        {
            public bool Enabled { get; set; } = false;
            public DataLogConfig(ConfigHelper helper)
            {
                this.Enabled = helper.GetBool("datalogger.enabled", false);
            }
        }

        public class CountersConfig
        {
            public Counters.DropCategory DropCategories { get; set; } = (Counters.DropCategory)15;
            public bool LogDropsToTotalCounters { get; set; } = false;
            public int MaterialsRarityFilter { get; set; } = 6;
            public CountersConfig(ConfigHelper helper)
            {
                DropCategories = (Counters.DropCategory)helper.GetInt("counters.dropCategories", 15);
                LogDropsToTotalCounters = helper.GetBool("counters.logDropsToTotal", false);
                MaterialsRarityFilter = helper.GetInt("counters.materialsRarityFilter", 6);
            }
        }

        public class LabConfig
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
            public LabConfig(ConfigHelper helper)
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
        }

        public class ProxyConfig
        {
            public int Port { get; set; } = 8081;
            public bool Secure { get; set; } = true;
            public string Blocklist { get; set; } = "";
            public bool ConnectionPooling { get; set; } = false;
            public bool AutoConfig { get; set; } = false;
            public ProxyConfig(ConfigHelper helper)
            {
                Port = helper.GetInt("proxy.port", 8081);
                Secure = helper.GetBool("proxy.secure", false);
                Blocklist = helper.GetString("proxy.blocklist", "");
                ConnectionPooling = helper.GetBool("proxy.connectionPooling", false);
                AutoConfig = helper.GetBool("proxy.autoconfig", false);
            }
        }

        public class SchedulerConfig
        {
            public int MaintenanceDoneHourUTC { get; set; } = 13;
            public SchedulerConfig(ConfigHelper helper)
            {
                MaintenanceDoneHourUTC = helper.GetInt("scheduler.maintenanceDoneHourUtc", 13);
            }
        }

        public ConsoleConfig Console { get; private set; }
        public UpdateConfig Updates { get; private set; }
        public ScreenConfig Screen { get; private set; }
        public AdbConfig Adb { get; private set; }
        public DataLogConfig DataLog { get; private set; }
        public CountersConfig Counters { get; private set; }
        public LabConfig Lab { get; private set; }
        public ProxyConfig Proxy { get; private set; }
        public SchedulerConfig Scheduler { get; private set; }

        public AppConfig(String configFile)
        {
            helper = new ConfigHelper(configFile);
            this.Console = new ConsoleConfig(helper);
            this.Updates = new UpdateConfig(helper);
            this.Screen = new ScreenConfig(helper);
            this.Adb = new AdbConfig(helper);
            this.DataLog = new DataLogConfig(helper);
            this.Counters = new CountersConfig(helper);
            this.Lab = new LabConfig(helper);
            this.Proxy = new ProxyConfig(helper);
            this.Scheduler = new SchedulerConfig(helper);
        }

        public void Save()
        {
            Screen.Save(helper);
            
        }

    }
}
