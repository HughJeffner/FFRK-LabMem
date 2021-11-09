using FFRK_Machines.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    public class LabConfiguration : MachineConfiguration
    {
        public bool AutoStart { get; set; } = false;
        public bool OpenDoors { get; set; } = true;
        public bool AvoidExploreIfTreasure { get; set; } = false;
        public bool AvoidPortal { get; set; } = true;
        public int WatchdogMinutes { get; set; } = 10;
        public bool RestartFailedBattle { get; set; } = false;
        public bool StopOnMasterPainting { get; set; } = false;
        public bool RestartLab { get; set; } = false;
        public bool UsePotions { get; set; } = false;
        public bool UseOldCrashRecovery { get; set; } = false;
        public bool UseLetheTears { get; set; } = false;
        public byte LetheTearsSlot { get; set; } = 0b11111;
        public int LetheTearsFatigue { get; set; } = 7;
        public bool UseTeleportStoneOnMasterPainting { get; set; } = false;
        public bool ScreenshotRadiantPainting { get; set; } = false;
        public bool EnemyBlocklistAvoidOptionOverride { get; set; } = false;
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, int> PaintingPriorityMap { get; set; } = new Dictionary<string, int>();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, TreasureFilter> TreasureFilterMap { get; set; } = new Dictionary<string, TreasureFilter>();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<EnemyBlocklistEntry> EnemyBlocklist { get; set; } = new List<EnemyBlocklistEntry>();
        public Dictionary<string, Timing> Timings { get; set; } = new Dictionary<string, Timing>();
        public LabConfiguration() {

            // Defaults
            this.Timings = GetDefaultTimings();
            this.PaintingPriorityMap = new Dictionary<string, int>
            {
                { "3", 1 },
                { "1.3", 2 },
                { "1.2", 3 },
                { "4", 4 },
                { "7", 5 },
                { "5", 6 },
                { "6", 7 },
                { "1.1", 8 },
                { "2", 9 }
            };
            this.TreasureFilterMap = new Dictionary<string, TreasureFilter>
            {
                {"5", new TreasureFilter(){ Priority=1, MaxKeys=1}},
                {"4", new TreasureFilter(){ Priority=1, MaxKeys=1}},
                {"3", new TreasureFilter(){ Priority=2, MaxKeys=1}},
                {"2", new TreasureFilter(){ Priority=0, MaxKeys=0}},
                {"1", new TreasureFilter(){ Priority=0, MaxKeys=0}}
            };
            this.EnemyBlocklist = new List<EnemyBlocklistEntry>
            {
                new EnemyBlocklistEntry(){Name="Alexander", Enabled=false},
                new EnemyBlocklistEntry(){Name="Atomos" ,Enabled=false},
                new EnemyBlocklistEntry(){Name="Diablos", Enabled=false},
                new EnemyBlocklistEntry(){Name="Lani & Scarlet Hair", Enabled=false},
                new EnemyBlocklistEntry(){Name="Lunasaurs", Enabled=false},
                new EnemyBlocklistEntry(){Name="Octomammoth", Enabled=false},
                new EnemyBlocklistEntry(){Name="Marilith", Enabled=false}
            };

        }

        public Dictionary<string, Timing> GetDefaultTimings()
        {
            Dictionary<string, Timing> defaults = new Dictionary<string, Timing>
            {
                { "Pre-AutoStart", new Timing() { Delay=10} },
                { "Inter-AutoStart", new Timing() { Delay=1000} },
                { "Post-AutoStart", new Timing() { Delay=0} },
                { "Pre-SelectPainting", new Timing()},
                { "Inter-SelectPainting", new Timing(){ Delay=1000 } },
                { "Post-SelectPainting", new Timing(){ Delay=0 } },
                { "Pre-RadiantPaintingScreenshot", new Timing(){ Delay=4000 } },
                { "Pre-SelectTreasure", new Timing() },
                { "Inter-SelectTreasure", new Timing() {Delay=2000 } }, 
                { "Post-SelectTreasure", new Timing() {Delay=0 } },
                { "Pre-Door", new Timing() },
                { "Post-Door", new Timing(){ Delay=1000} },
                { "Pre-MoveOn", new Timing() },
                { "Post-MoveOn", new Timing() { Delay=1000 } },
                { "Post-MoveOn-Portal", new Timing() { Delay=5000} },
                { "Pre-StartBattle", new Timing() { Delay=0} },
                { "Pre-StartBattle-Fatigue", new Timing() { Delay=20000} },
                { "Inter-StartBattle", new Timing() { Delay=500} },
                { "Post-StartBattle", new Timing() { Delay=0} },
                { "Post-Battle", new Timing(){ Delay=7000 } },
                { "Pre-ConfirmPortal", new Timing(){ Delay=5000 } },
                { "Post-ConfirmPortal", new Timing(){ Delay=2000 } },
                { "Pre-LetheTears", new Timing(){ Delay=4000 } },
                { "Inter-LetheTears", new Timing(){ Delay=2000 } },
                { "Inter-LetheTears-Unit", new Timing(){ Delay=500 } },
                { "Post-LetheTears", new Timing(){ Delay=0 } },
                { "Pre-TeleportStone", new Timing(){ Delay=2000 } },
                { "Inter-TeleportStone", new Timing(){ Delay=2000 } },
                { "Post-TeleportStone", new Timing(){ Delay=0 } },
                { "Pre-RestartLab", new Timing(){ Delay=60000 } },
                { "Inter-RestartLab", new Timing(){ Delay=5000 } },
                { "Inter-RestartLab-Stamina", new Timing(){ Delay=2000 } },
                { "Post-RestartLab", new Timing(){ Delay=4000 } },
                { "Pre-RestartFFRK", new Timing(){ Delay=5000 } },
                { "Inter-RestartFFRK", new Timing(){ Delay=4000 } },
                { "Inter-RestartFFRK-Timeout", new Timing(){ Delay=180000 } },
                { "Post-RestartFFRK", new Timing(){ Delay=0 } },
                { "Pre-RestartBattle", new Timing(){ Delay=5000 } },
                { "Inter-RestartBattle", new Timing(){ Delay=2000 } },
                { "Post-RestartBattle", new Timing(){ Delay=0 } },
            };
            return defaults;
        }

        public class TreasureFilter
        {
            public int Priority { get; set; }
            public int MaxKeys { get; set; }
        }

        public class EnemyBlocklistEntry
        {
            public string Name { get; set; }
            public bool Enabled { get; set; } = false;
            public override string ToString()
            {
                return Name;
            }
        }

        public class Timing
        {
            private static Random rng = new Random();
            public int Delay { get; set; } = 5000;
            public int Jitter { get; set; } = 0;
            public int DelayWithJitter
            {
                get
                {
                    return rng.Next(this.Delay, this.Delay + this.Jitter);
                }
            }
            public Task Wait(CancellationToken cancellationToken)
            {
                return Task.Delay(this.DelayWithJitter, cancellationToken);
            }
        }
    }
}
