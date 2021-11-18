﻿using System;
using System.IO;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
using FFRK_Machines;
using FFRK_Machines.Machines;

namespace FFRK_LabMem.Machines
{
    public class LabController : MachineController<Lab, Lab.State, Lab.Trigger, LabConfiguration>
    {

        public static async Task<LabController> CreateAndStart(ConfigHelper config)
        {
            
            // Create instance
            var ret = new LabController();

            // Validate config file
            var configFilePath = config.GetString("lab.configFile", "Config/lab.balanced.json");
            if (!File.Exists(configFilePath)){
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not load {0}!", configFilePath);
                return ret;
            }

            // Counters
            await Data.Counters.Initalize(ret);

            // Start it
            await ret.Start(
                adbPath: config.GetString("adb.path", "adb.exe"),
                adbHost: config.GetString("adb.host", "127.0.0.1:7555"),
                proxyPort: config.GetInt("proxy.port", 8081),
                proxySecure: config.GetBool("proxy.secure", false),
                proxyBlocklist: config.GetString("proxy.blocklist",""),
                proxyConnectionPooling: config.GetBool("proxy.connectionpooling", false),
                proxyAutoConfig: config.GetBool("proxy.autoconfig", false),
                configFile: config.GetString("lab.configFile", "Config/lab.balanced.json"),
                topOffset: config.GetInt("screen.topOffset", -1),
                bottomOffset: config.GetInt("screen.bottomOffset", -1),
                consumers: 2);
            
            // Auto-detect offsets
            if (ret.Adb != null && ret.Adb.HasDevice && config.GetInt("screen.topOffset", -1) == -1)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Screen offsets not set up, press [Alt+O] to detect them once FFRK is on the Title Screen");
            }

            // Scheduler
            await Services.Scheduler.Default(ret).Start();
            
            return ret;
        }
        protected override Lab CreateMachine(LabConfiguration config)
        {
            var ch = new ConfigHelper();
            config.WatchdogHangMinutes = ch.GetInt("lab.watchdogHangMinutes", 10);
            config.WatchdogCrashSeconds = ch.GetInt("lab.watchdogCrashSeconds", 30);
            return new Lab(this.Adb, config);
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

        public void ManualFFRKRestart()
        {

            if (Enabled)
            {

                Task.Run(async () =>
                {
                    try
                    {
                        await this.Machine.ManualFFRKRestart();
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                    }

                });
            }

        }
    }
}
