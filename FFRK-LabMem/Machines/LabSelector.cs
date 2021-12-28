using FFRK_LabMem.Data;
using FFRK_LabMem.Services;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    class LabSelector
    {

        public Lab Lab;
        private LabConfiguration Config => Lab.Config;
        private Adb Adb => Lab.Adb;
        protected Random rng = new Random();

        public LabSelector(Lab lab)
        {
            Lab = lab;
        }

        public async Task<int> GetPaintingPriority(JToken painting, bool isTreasure, bool isExplore, int total, bool isLastFloor)
        {

            // Type as string
            var type = painting["type"].ToString();

            // Radiant painting
            if ((bool)painting["is_special_effect"])
            {
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, "Radiant painting detected!: {0}", painting["name"]);
                ColorConsole.WriteLine(ConsoleColor.DarkMagenta, new string('*', 60));
                if (Config.ScreenshotRadiantPainting)
                {
                    await LabTimings.Delay("Pre-RadiantPaintingScreenshot", Lab.CancellationToken);
                    await Adb.SaveScrenshot(String.Format("radiant_{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss")), Lab.CancellationToken);
                }
                await Counters.FoundRadiantPainting();
                var rPriority = 0;
                if (Config.PaintingPriorityMap.ContainsKey("R")) rPriority = Config.PaintingPriorityMap["R"];
                return rPriority;
            }

            // Combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();

                // Enemy blocklist
                var enemyName = painting["dungeon"]["captures"][0]["tip_battle"]["title"].ToString();
                if (Config.EnemyBlocklist.Any(b => b.Enabled && enemyName.ToLower().Contains(b.Name.ToLower())))
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding due to blocklist: {0}", enemyName);
                    return (Config.EnemyBlocklistAvoidOptionOverride ? 512 : 64);
                }
            }

            // Portal avoidance options
            if (type.Equals("6")){

                // There's a treasure visible but picked a portal
                if (Config.AvoidPortal && isTreasure)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return 256;
                }

                // There's a explore visible but picked a portal
                if (Config.AvoidPortalIfExplore && isExplore)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return 256;
                }

                // There's more paintings to reveal but picked a portal
                if (Config.AvoidPortalIfExplore && total > 9)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return 256;
                }

            }

            // There's a treasure visible but explore (unless last floor)
            if (type.Equals("4") && Config.AvoidExploreIfTreasure && isTreasure && !isLastFloor)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding explore due to option");
                return 128;
            }

            // Lookup or default
            if (Config.PaintingPriorityMap.ContainsKey(type))
            {
                return Config.PaintingPriorityMap[type];
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Unknown painting id: {0}", type);
                return 1024;
            }

        }

        public LabConfiguration.TreasureFilter GetTreasureFilter(JToken treasure)
        {

            var type = treasure.ToString().Substring(0, 1);

            if (Config.TreasureFilterMap.ContainsKey(type))
            {
                return Config.TreasureFilterMap[type];
            }
            else
            {
                if (!type.Equals("0")) ColorConsole.WriteLine(ConsoleColor.DarkRed, "Unknown treasure filter id: {0}", type);
                return new LabConfiguration.TreasureFilter() { MaxKeys = 0, Priority = 0 };
            }

        }

        public int GetPartyIndex()
        {

            switch (Lab.Config.PartyIndex)
            {
                case LabConfiguration.PartyIndexOption.Team1:
                case LabConfiguration.PartyIndexOption.Team2:
                case LabConfiguration.PartyIndexOption.Team3:
                    return (int)Lab.Config.PartyIndex;
                case LabConfiguration.PartyIndexOption.Random:
                    return rng.Next(0, 2);
                case LabConfiguration.PartyIndexOption.LowestFatigue:
                    var item = Lab.FatigueInfo
                        .Select(p => p)
                        .OrderBy(p => p.Sum(f=>f.Fatigue))   
                        .ThenBy(p => rng.Next())            
                        .FirstOrDefault();
                    if (item!= default) return Lab.FatigueInfo.IndexOf(item);
                    return 0;
                default:
                    return 0;
            }

        }

    }
}
