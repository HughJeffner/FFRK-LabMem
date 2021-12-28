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

            // There's a treasure or explore visible or more paintings not visible yet, but picked a portal
            if (type.Equals("6") && this.Config.AvoidPortal && (isTreasure || isExplore || (total > 9)))
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding portal due to option");
                return 256;
            }

            // There's a treasure visible but explore (unless last floor)
            if (type.Equals("4") && this.Config.AvoidExploreIfTreasure && isTreasure && !isLastFloor)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Avoiding explore due to option");
                return 128;
            }

            // Lookup or default
            if (this.Config.PaintingPriorityMap.ContainsKey(type))
            {
                return this.Config.PaintingPriorityMap[type];
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

    }
}
