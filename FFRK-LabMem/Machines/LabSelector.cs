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
            var maxPriority = Config.PaintingPriorityMap.Values.Max() * 10;
            var portalPriority = Config.PaintingPriorityMap.Where(p => p.Key.Equals("6")).FirstOrDefault().Value * 10;
            var masterPriority = Config.PaintingPriorityMap.Where(p => p.Key.Equals("2")).FirstOrDefault().Value * 10;

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
                return rPriority * 10;
            }

            // Combatant (1)
            if (type.Equals("1"))
            {
                type += "." + painting["display_type"].ToString();

                // Enemy blocklist
                var enemyName = painting["dungeon"]["captures"][0]["tip_battle"]["title"].ToString();
                var enemyEntry = Config.EnemyPriorityList.FirstOrDefault(b => b.Enabled && enemyName.ToLower().Contains(b.Name.ToLower()));
                if (enemyEntry != null)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Adjusting priority {0:+#;-#;0} due to blocklist: {1}",
                        enemyEntry.PriorityAdjust,
                        enemyName);
                    var combatants = Config.PaintingPriorityMap.Where(p => p.Key.StartsWith("1."));
                    var highest = combatants.OrderByDescending(p2=>p2.Value).First().Value * 10;
                    var lowest = combatants.OrderBy(p2=>p2.Value).First().Value * 10;
                    var priority = Config.PaintingPriorityMap[type] * 10;
                    if (enemyEntry.PriorityAdjust > 0) priority = highest + enemyEntry.PriorityAdjust;
                    if (enemyEntry.PriorityAdjust < 0) priority = lowest + enemyEntry.PriorityAdjust;
                    return (Config.EnemyBlocklistAvoidOptionOverride ? maxPriority + 10 : priority);
                }
            }

            // Master avoidance options
            if (type.Equals("2"))
            {

                // There's a treasure visible
                if (Config.AvoidMasterIfTreasure && isTreasure)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding master due to option");
                    return maxPriority + 5;
                }

                // There's a explore visible
                if (Config.AvoidMasterIfExplore && isExplore)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding master due to option");
                    return maxPriority + 5;
                }

                // There's more paintings to reveal
                if (Config.AvoidMasterIfMore && total > 9)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding master due to option");
                    return maxPriority + 5;
                }

            }

            // Portal avoidance options
            if (type.Equals("6")){

                // There's a treasure visible but picked a portal
                if (Config.AvoidPortal && isTreasure)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return maxPriority + 5;
                }

                // There's a explore visible but picked a portal
                if (Config.AvoidPortalIfExplore && isExplore)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return maxPriority + 5;
                }

                // There's more paintings to reveal but picked a portal
                if (Config.AvoidPortalIfExplore && total > 9)
                {
                    ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                    return maxPriority + 5;
                }

            }

            // There's a treasure visible but explore (unless last floor)
            if (type.Equals("4") && Config.AvoidExploreIfTreasure && isTreasure && !isLastFloor)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding explore due to option");
                return maxPriority +1;
            }

            // Lookup or default
            if (Config.PaintingPriorityMap.ContainsKey(type))
            {
                return Config.PaintingPriorityMap[type] * 10;
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkRed, "Unknown painting id: {0}", type);
                return maxPriority + 100;
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
                case LabConfiguration.PartyIndexOption.Random12:
                    return rng.Next(0, 1);
                case LabConfiguration.PartyIndexOption.RandomAny:
                    return rng.Next(0, 2);
                case LabConfiguration.PartyIndexOption.LowestFatigue12:
                    var item12 = Lab.FatigueInfo
                        .Take(2)
                        .Select(p => p)
                        .OrderBy(p => p.Sum(f => f.Fatigue))
                        .ThenBy(p => rng.Next())
                        .FirstOrDefault();
                    if (item12 != default) return Lab.FatigueInfo.IndexOf(item12);
                    return 0;
                case LabConfiguration.PartyIndexOption.LowestFatigueAny:
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
