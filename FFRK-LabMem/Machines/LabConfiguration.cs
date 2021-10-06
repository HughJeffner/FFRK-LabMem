using FFRK_Machines.Machines;
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
        public Dictionary<String, int> PaintingPriorityMap { get; set; } = new Dictionary<string, int>();
        public Dictionary<String, TreasureFilter> TreasureFilterMap { get; set; } = new Dictionary<string, TreasureFilter>();
        public int WatchdogMinutes { get; set; } = 10;
        public bool RestartFailedBattle { get; set; } = false;
        public bool StopOnMasterPainting { get; set; } = true;
        public bool RestartLab { get; set; } = false;
        public bool UsePotions { get; set; } = false;
        public bool UseOldCrashRecovery { get; set; } = false;
        public bool UseLetheTears { get; set; } = false;
        public byte LetheTearsSlot { get; set; } = 0b11111;
        public int LetheTearsFatigue { get; set; } = 8;

        public LabConfiguration() {}

        public class TreasureFilter
        {
            public int Priority { get; set; }
            public int MaxKeys { get; set; }
        }

    }
}
