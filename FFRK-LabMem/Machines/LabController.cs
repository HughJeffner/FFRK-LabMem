using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
using FFRK_Machines;
using FFRK_Machines.Machines;
using FFRK_Machines.Extensions;

namespace FFRK_LabMem.Machines
{
    class LabController : MachineController<Lab, Lab.State, Lab.Trigger, Lab.Configuration>
    {

        public static async Task<LabController> CreateAndStart(ConfigHelper config)
        {
            
            // Create instance
            var ret = new LabController();
            
            // Start it
            await ret.Start(debug: config.GetBool("console.debug", false),
                adbPath: config.GetString("adb.path", "adb.exe"),
                adbHost: config.GetString("adb.host", "127.0.0.1:7555"),
                proxyPort: config.GetInt("proxy.port", 8081),
                configFile: config.GetString("lab.configFile", "Config/lab.balanced.json"),
                topOffset: config.GetInt("screen.topOffset", -1),
                bottomOffset: config.GetInt("screen.bottomOffset", -1),
                unkownState: Lab.State.Unknown);
            
            // Auto-detect offsets
            if (ret.Adb != null && ret.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets not set up, press [Alt+O] to detect them once FFRK is on the Title Screen");
            }
            
            return ret;
        }

        public async void AutoDetectOffsets(ConfigHelper config) {

            if (this.Adb != null && this.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detecting screen offsets...");
                var offsets = await GetOffsets(2000);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detected offset t:{0}, b:{1}, saving to .config", offsets.Item1, offsets.Item2);
                this.Adb.TopOffset = offsets.Item1;
                this.Adb.BottomOffset = offsets.Item2;
                config.SetValue("screen.topOffset", offsets.Item1.ToString());
                config.SetValue("screen.bottomOffset", offsets.Item2.ToString());
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets already set ({0},{1}).  Please set them in the .config file to '-1' for auto-detect", this.Adb.TopOffset, this.Adb.BottomOffset);
            }

        }

        protected override Lab CreateMachine(Lab.Configuration config)
        {
            return new Lab(this.Adb, config);
        }

        protected async Task<Tuple<int, int>> GetOffsets(int threshold)
        {

            int topOffset = 0;
            int bottomOffset = 0;

            // Screen size
            var size = await this.Adb.GetScreenSize();

            // Coordinates from top of screen to bottom
            var coords = new List<Tuple<int, int>>();
            for (int i = 0; i < size.Height; i++)
            {
                coords.Add(new Tuple<int, int>(size.Width / 2, i));
            }

            // Get color values
            var results = await this.Adb.GetPixelColorXY(coords, System.Threading.CancellationToken.None);

            // Target color gray
            var target = System.Drawing.ColorTranslator.FromHtml("#151515");

            // Hold matches
            var matches = new List<int>();
            int itemIndex = 0;

            // Inspect each item
            foreach (var item in results)
            {
                // Distance to target
                var d = item.GetDistance(target);

                // If below threshold add to matches
                if (d < threshold) matches.Add(itemIndex);
                itemIndex++;

            }

            // Inspect matches starting from 0, if a jump over 1 occurs then top offset
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i] != i)
                {
                    topOffset = i-1;
                    break;
                }
            }

            // Inspect matches starting from last match, if a jump over 1 occurs then bottom offset
            for (int i = matches.Count-1; i>0; i--)
            {
                if (matches[i] != (size.Height-1) - (matches.Count-i-1))
                {
                    bottomOffset = matches.Count - 1 - i;
                    break;
                }
            }

            // Sanity check
            if (topOffset < 0) topOffset = 0;
            if (bottomOffset < 0) bottomOffset = 0;

            return new Tuple<int,int>(topOffset,bottomOffset);

        }

    }
}
