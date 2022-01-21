﻿using FFRK_Machines.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FFRK_LabMem.Machines
{
    public class LabConfiguration : MachineConfiguration
    {

        protected override async void Migrate(String oldVersion, String newVersion)
        {
            // Ensure radiant painting in priority list
            if (!PaintingPriorityMap.ContainsKey("R")) PaintingPriorityMap.Add("R", 0);

            // First time migrate on 7.0 sets all timings to default
            if (!oldVersion.Equals("7.0.0.0") && newVersion.Equals("7.0.0.0")) await LabTimings.ResetToDefaults();
        }

        protected override string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public enum PartyIndexOption
        {
            Team1,
            Team2,
            Team3,
            LowestFatigueAny,
            LowestFatigue12,
            RandomAny,
            Random12,
        }
        public enum CompleteMissionOption
        {
            None,
            DefeatMasterPainting,
            QuickExplore
        }
        public bool AutoStart { get; set; } = false;
        public bool OpenDoors { get; set; } = true;
        public bool AvoidExploreIfTreasure { get; set; } = false;
        public bool AvoidPortal { get; set; } = true;
        public bool AvoidPortalIfExplore { get; set; } = true;
        public bool AvoidPortalIfMore { get; set; } = true;
        public bool AvoidMasterIfTreasure { get; set; } = true;
        public bool AvoidMasterIfExplore { get; set; } = true;
        public bool AvoidMasterIfMore { get; set; } = true;
        public bool RestartFailedBattle { get; set; } = false;
        public bool StopOnMasterPainting { get; set; } = false;
        public bool RestartLab { get; set; } = false;
        public bool UsePotions { get; set; } = false;
        public bool UseOldCrashRecovery { get; set; } = false;
        public bool UseLetheTears { get; set; } = false;
        public bool LetheTearsMasterOnly { get; set; } = false;
        [Obsolete]
        public byte LetheTearsSlot { internal get; set; } = 0;
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<byte> LetheTearsSlots { get; set; } = new List<byte>()
        {
            {0b11111},
            {0b00000},
            {0b00000}
        };
        public int LetheTearsFatigue { get; set; } = 7;
        public PartyIndexOption PartyIndex { get; set; } = PartyIndexOption.Team1;
        public bool UseTeleportStoneOnMasterPainting { get; set; } = false;
        public bool ScreenshotRadiantPainting { get; set; } = false;
        public bool EnemyBlocklistAvoidOptionOverride { get; set; } = false;
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, int> PaintingPriorityMap { get; set; } = new Dictionary<string, int>();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, TreasureFilter> TreasureFilterMap { get; set; } = new Dictionary<string, TreasureFilter>();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<EnemyBlocklistEntry> EnemyBlocklist { get; set; } = new List<EnemyBlocklistEntry>();
        public CompleteMissionOption CompleteDailyMission { get; set; } = CompleteMissionOption.None;

        public LabConfiguration() {

            // Defaults
            this.PaintingPriorityMap = new Dictionary<string, int>
            {
                { "R", 0 },
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
                new EnemyBlocklistEntry(){Name="Marilith", Enabled=false},
                new EnemyBlocklistEntry(){Name="Behemoth", Enabled=false, PriorityAdjust=-1},
                new EnemyBlocklistEntry(){Name="Nidhogg", Enabled=false, PriorityAdjust=-1},
                new EnemyBlocklistEntry(){Name="Odin", Enabled=false, PriorityAdjust=-1},
                new EnemyBlocklistEntry(){Name="Faeryl", Enabled=false, PriorityAdjust=-1},
                new EnemyBlocklistEntry(){Name="Ultima Weapon", Enabled=false, PriorityAdjust=-1},
                new EnemyBlocklistEntry(){Name="Deathclaws", Enabled=false, PriorityAdjust=-1},
            };

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
            public int PriorityAdjust { get; set; } = 1;
            public override string ToString()
            {
                return Name;
            }
        }
        
    }
}
