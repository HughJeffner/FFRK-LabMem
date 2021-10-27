using System;
using System.Collections.Generic;

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
            {"7", "Restoration Painting"}
        };

        public static Dictionary<String, String> Treasures = new Dictionary<string, string>() {
            {"5", "Hero Equipment"},
            {"4", "Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone"},
            {"3", "6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion"},
            {"2", "6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom"},
            {"1", "5* Orbs, 5* Motes"}
        };

        public static List<AdbHostItem> AdbHosts = new List<AdbHostItem>() {
            new AdbHostItem { Name = "MuMu", Value = "127.0.0.1:7555"} ,
            new AdbHostItem { Name = "Nox 5", Value = "127.0.0.1:62001"} ,
            new AdbHostItem { Name = "Nox", Value = "127.0.0.1:5037"} ,
            new AdbHostItem { Name = "MEmu", Value = "127.0.0.1:21503"} ,
            new AdbHostItem { Name = "MEmu Instance 2", Value = "127.0.0.1:21513"} ,
            new AdbHostItem { Name = "MEmu Instance 3", Value = "127.0.0.1:21523"} ,
            new AdbHostItem { Name = "LD Player", Value = "127.0.0.1:5555"}
        };

    }
}
