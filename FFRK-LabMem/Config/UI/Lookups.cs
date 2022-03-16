using FFRK_LabMem.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using FFRK_Machines.Services.Notifications;
using System.ComponentModel;

namespace FFRK_LabMem.Config.UI
{
    class Lookups
    {

        public static Dictionary<String, String> Paintings = new Dictionary<string, string>() {
            {"1.1", "Combatant Painting (Green)"},
            {"1.2", "Combatant Painting (Orange)"},
            {"1.3", "Combatant Painting (Red)"},
            {"2", "Master Painting"},
            {"3", "Treasure Painting"},
            {"4", "Exploration Painting"},
            {"5", "Onslaught Painting"},
            {"6", "Portal Painting"},
            {"7", "Restoration Painting"},
            {"R", "Radiant Painting"}
        };

        public static Dictionary<String, String> Treasures = new Dictionary<string, string>() {
            {"5", "Hero Equipment"},
            {"4", "Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone"},
            {"3", "6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion"},
            {"2", "6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom"},
            {"1", "5* Orbs, 5* Motes"}
        };

        public static BindingList<AdbHostItem> AdbHosts = new BindingList<AdbHostItem>() {
            new AdbHostItem { Name = "MuMu", Value = "127.0.0.1:7555"} ,
            new AdbHostItem { Name = "Nox", Value = "127.0.0.1:62001"} ,
            new AdbHostItem { Name = "Nox Alt 1", Value = "127.0.0.1:62025"} ,
            new AdbHostItem { Name = "Nox Alt 2", Value = "127.0.0.1:62026"} ,
            new AdbHostItem { Name = "Nox Alt 3", Value = "127.0.0.1:62027"} ,
            new AdbHostItem { Name = "MEmu", Value = "127.0.0.1:21503"} ,
            new AdbHostItem { Name = "MEmu Instance 2", Value = "127.0.0.1:21513"} ,
            new AdbHostItem { Name = "MEmu Instance 3", Value = "127.0.0.1:21523"} ,
            new AdbHostItem { Name = "Other", Value = "127.0.0.1:5555"}
        };

        public static Dictionary<String, String> Timings = new Dictionary<string, string>() {
            {"Pre-SelectPainting", "Delay before selecting a painting"},
            {"Inter-SelectPainting", "Delay between selecting a painting and confirming it"},
            {"Pre-SelectTreasure", "Delay before selecting the first treasure"},
            {"Pre-Door", "Delay before either opening or leaving a sealed door"},
            {"Pre-MoveOn", "Delay before moving on after most explore results"},
            {"Post-Battle","Delay before pressing skip after a battle ends" },
            {"Pre-RestartLab","Delay before starting a new lab run when degenerate mode is enabled" },
            {"Inter-RestartFFRK-Timeout","How long to wait when restarting FFRK before giving up" },
            {"Inter-RestartLab-Stamina","DEPRECATED: Use X-Stamina timings instead" },
        };

        public static Dictionary<String, String> Blocklist = new Dictionary<string, string>() {
            {"Alexander", "High resistance"},
            {"Atomos", "Slowga, High Damage"},
            {"Diablos", "High Damage"},
            {"Lani & Scarlet Hair", "Damage Sponge"},
            {"Lunasaurs", "Reflect"},
            {"Octomammoth","Reflect" },
            {"Marilith", "High resistance, Blind" }
        };

        public static Dictionary<String, String> Counters = new Dictionary<string, string>() {
            {"LabRunsCompleted", "Lab Runs Completed"},
            {"PaintingsSelected", "Paintings Selected"},
            {"BattlesWon", "Battles Won"},
            {"TreasuresOpened", "Treasures Opened"},
            {"RadiantPaintings", "Radiant Paintings Found"},
            {"MagicPots", "Magic Pots Found"},
            {"UsedTears", "Lethe Tears Used"},
            {"UsedKeys", "Magic Keys Used"},
            {"UsedTeleportStones", "Teleport Stones Used"},
            {"UsedStaminaPots", "Stamina Pots Used"},
            {"PulledInPortal", "Pulled Into Portals"},
            {"FFRKCrashes", "FFRK Crashed"},
            {"FFRKHangs", "FFRK Hangs"},
            {"FFRKRecoveries", "FFRK Recovered"},
            {"FFRKRestarts", "FFRK Restarted"},
            {"HeroEquipmentGot", "Hero Equipment Found"},
            {"EnemyIsUponYou", "The Enemy is Upon You!"},
            {"QuickExplores", "Quick Explores"},
        };

        public static Dictionary<Counters.DropCategory, String> DropCategories = new Dictionary<Counters.DropCategory, string>() {
            {Data.Counters.DropCategory.EQUIPMENT, "Hero Equipment"},
            {Data.Counters.DropCategory.LABYRINTH_ITEM, "Labyrinth Items (Bookmarks, Keys...)"},
            {Data.Counters.DropCategory.COMMON, "Anima Lenses"},
            {Data.Counters.DropCategory.SPHERE_MATERIAL, "Motes"},
            {Data.Counters.DropCategory.ABILITY_MATERIAL, "Crystals/Orbs"},
            {Data.Counters.DropCategory.EQUIPMENT_SP_MATERIAL,"Upgrade Materials" },
            {Data.Counters.DropCategory.HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL, "Rat Tails" },
            {Data.Counters.DropCategory.GROW_EGG, "Growth Eggs" },
            {Data.Counters.DropCategory.BEAST_FOOD, "Arcana" },
            {Data.Counters.DropCategory.RECORD_MATERIA, "Record Materia" }
        };

        public static Dictionary<String, Counters.DropCategory> DropCategoriesInverse = DropCategories.ToDictionary((i) => i.Value, (i) => i.Key);
        

        public static Dictionary<Notifications.EventType, String> NotificationEvents = new Dictionary<Notifications.EventType, string>() {
            {Notifications.EventType.LAB_COMPLETED, "Lab Completed"},
            {Notifications.EventType.LAB_FAULT, "Lab Fault"},
        };

        public static Dictionary<String, Notifications.EventType> NotificationEventsInverse = NotificationEvents.ToDictionary((i) => i.Value, (i) => i.Key);

    }
}
