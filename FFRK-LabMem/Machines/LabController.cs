using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
using FFRK_Machines;
using FFRK_Machines.Machines;
using FFRK_Machines.Extensions;

namespace FFRK_LabMem.Machines
{
    public class LabController : MachineController<Lab, Lab.State, Lab.Trigger, Lab.Configuration>
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
                proxySecure: config.GetBool("proxy.secure", false),
                proxyBlocklist: config.GetString("proxy.blocklist",""),
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
                var offsets = await this.Adb.GetOffsets("#151515", 2000, System.Threading.CancellationToken.None);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detected offset t:{0}, b:{1}, saving to .config", offsets.Item1, offsets.Item2);
                this.Adb.TopOffset = offsets.Item1;
                this.Adb.BottomOffset = offsets.Item2;
                config.SetValue("screen.topOffset", offsets.Item1);
                config.SetValue("screen.bottomOffset", offsets.Item2);
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

    }
}
