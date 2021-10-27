using FFRK_Machines.Machines;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FFRK_LabMem.Machines
{
    public class LabConfiguration : MachineConfiguration
    {
        public bool OpenDoors { get; set; } = true;
        public bool AvoidExploreIfTreasure { get; set; } = true;
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
        public Dictionary<string, int> PaintingPriorityMap { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, TreasureFilter> TreasureFilterMap { get; set; } = new Dictionary<string, TreasureFilter>();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<EnemyBlocklistEntry> EnemyBlocklist { get; set; } = new List<EnemyBlocklistEntry>();
        public Dictionary<string, int> Timings { get; set; } = new Dictionary<string, int>();
        public LabConfiguration() {

            // Defaults
            this.Timings = GetDefaultTimings();
            this.EnemyBlocklist = new List<EnemyBlocklistEntry>
            {
                new EnemyBlocklistEntry(){Name="Alexander",Enabled=false},
                new EnemyBlocklistEntry(){Name="Atomos",Enabled=false},
                new EnemyBlocklistEntry(){Name="Octomammoth",Enabled=false},
                new EnemyBlocklistEntry(){Name="Lunasaurs",Enabled=true}
            };

        }

        public Dictionary<string, int> GetDefaultTimings()
        {
            Dictionary<string, int> defaults = new Dictionary<string, int>
            {
                { "Pre-SelectPainting", 5000 },
                { "Inter-SelectPainting", 1000 },
                { "Pre-SelectTreasure", 5000 },
                { "Pre-Door", 5000 },
                { "Pre-MoveOn", 5000 },
                { "Post-Battle", 7000 }
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
    }
}
