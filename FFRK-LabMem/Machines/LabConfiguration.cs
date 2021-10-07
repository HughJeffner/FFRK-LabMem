﻿using FFRK_Machines.Machines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FFRK_LabMem.Machines
{
    public class LabConfiguration : MachineConfiguration
    {

        public bool OpenDoors { get; set; } = true;
        public bool AvoidExploreIfTreasure { get; set; } = true;
        public bool AvoidPortal { get; set; } = true;
        public int WatchdogMinutes { get; set; } = 10;
        public bool RestartFailedBattle { get; set; } = false;
        public bool StopOnMasterPainting { get; set; } = true;
        public bool RestartLab { get; set; } = false;
        public bool UsePotions { get; set; } = false;
        public bool UseOldCrashRecovery { get; set; } = false;
        public bool UseLetheTears { get; set; } = false;
        public byte LetheTearsSlot { get; set; } = 0b11111;
        public int LetheTearsFatigue { get; set; } = 7;
        public bool UseTeleportStoneOnMasterPainting { get; set; } = false;
        public Dictionary<String, int> PaintingPriorityMap { get; set; } = new Dictionary<string, int>();
        public Dictionary<String, TreasureFilter> TreasureFilterMap { get; set; } = new Dictionary<string, TreasureFilter>();
        public Dictionary<String, int> Timings { get; set; } = new Dictionary<string, int>();

        public LabConfiguration() {

            // Default timings
            this.Timings = GetDefaultTimings();

        }

        public Dictionary<String, int> GetDefaultTimings()
        {
            Dictionary<String, int> defaults = new Dictionary<string, int>
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

    }
}
