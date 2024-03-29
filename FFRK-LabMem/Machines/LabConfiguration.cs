﻿using FFRK_Machines.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FFRK_LabMem.Machines
{
    public class LabConfiguration : MachineConfiguration
    {

        protected override void Migrate(String oldVersion, String newVersion)
        {
            // Ensure radiant painting in priority list
            if (!PaintingPriorityMap.ContainsKey("R")) PaintingPriorityMap.Add("R", 0);

            // First time migrate on 7.3 sets Inter-SelectPainting to default
            if (!oldVersion.Equals("7.3.0.0") && newVersion.Equals("7.3.0.0")) LabTimings.Timings["Inter-SelectPainting"].Delay = LabTimings.DefaultTimings["Inter-SelectPainting"].Delay;

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
            InstaBattle
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
        public List<EnemyPriority> EnemyPriorityList { get; set; } = new List<EnemyPriority>();
        public CompleteMissionOption CompleteDailyMission { get; set; } = CompleteMissionOption.None;
        public bool BoostRestoreEnabled { get; set; } = false;
        public int BoostRestoreFatigue { get; set; } = 7;
        public bool WaitForStamina { get; set; } = false;
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
            this.EnemyPriorityList = new List<EnemyPriority>
            {
                new EnemyPriority(){Name="Alexander", Enabled=false},
                new EnemyPriority(){Name="Atomos" ,Enabled=false},
                new EnemyPriority(){Name="Diablos", Enabled=false},
                new EnemyPriority(){Name="Lani & Scarlet Hair", Enabled=false},
                new EnemyPriority(){Name="Lunasaurs", Enabled=false},
                new EnemyPriority(){Name="Octomammoth", Enabled=false},
                new EnemyPriority(){Name="Marilith", Enabled=false},
                new EnemyPriority(){Name="Behemoth", Enabled=false, PriorityAdjust=-1},
                new EnemyPriority(){Name="Nidhogg", Enabled=false, PriorityAdjust=-1},
                new EnemyPriority(){Name="Odin", Enabled=false, PriorityAdjust=-1},
                new EnemyPriority(){Name="Faeryl", Enabled=false, PriorityAdjust=-1},
                new EnemyPriority(){Name="Ultima Weapon", Enabled=false, PriorityAdjust=-1},
                new EnemyPriority(){Name="Deathclaws", Enabled=false, PriorityAdjust=-1},
            };

        }

        public class TreasureFilter
        {
            public int Priority { get; set; }
            public int MaxKeys { get; set; }
        }

        public class EnemyPriority
        {
            public string Name { get; set; }
            public bool Enabled { get; set; } = false;
            public int PriorityAdjust { get; set; } = 1;
            public List<int> Parties { get; set; } = new List<int>();
            public override string ToString()
            {
                return Name;
            }
        }
        
    }
}
