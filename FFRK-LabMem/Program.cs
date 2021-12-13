﻿using System;
using FFRK_LabMem.Config;
using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Data.UI;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using FFRK_Machines;
using FFRK_Machines.Services;

namespace FFRK_LabMem
{
    class Program
    {
        static void Main(string[] args)
        {

            // Listen for console exit
            ConsoleTasks.ListenForExit(OnConsoleExit);
            ConsoleTasks.DisableQuickEditMode();
            Console.TreatControlCAsInput = true;

            // Get Configuration
            var configFile = (args.Length > 0) ? args[0] : null;
            var config = new ConfigHelper(configFile);

            // Console
            ColorConsole.Timestamps = config.GetBool("console.timestamps", true);
            ColorConsole.DebugCategories = (ColorConsole.DebugCategory)config.GetInt("console.debugCategories", 0);
            ColorConsole.LogBuffer.Enabled = config.GetBool("console.logging", false);
            
            // Version check
            var versionCode = Updates.GetVersionCode("beta");
            var versionTitle = String.Format("{0} {1}", Updates.GetName(), versionCode);
            ColorConsole.WriteLine(versionTitle);
            Console.Title = versionTitle;
            if (config.GetBool("updates.checkForUpdates", false))
                _ = Updates.Check(config.GetBool("updates.includePrerelease", false));


            // Controller
            LabController controller = LabController.CreateAndStart(config).Result;

            // Enable visual styles in forms
            System.Windows.Forms.Application.EnableVisualStyles();

            // Ad-hoc command loop
            Console.WriteLine("Press 'D' to Disable, 'E' to Enable, 'C' for Config, 'S' for Stats, 'Ctrl+X' to Exit");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control) break;
                if (key.Key == ConsoleKey.E) controller.Enable();
                if (key.Key == ConsoleKey.D) controller.Disable();
                if (key.Key == ConsoleKey.H) Tray.MinimizeTo(key.Modifiers, controller);
                if (key.Key == ConsoleKey.C && key.Modifiers == 0) ConfigForm.CreateAndShow(config, controller);
                if (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control) Console.Clear();
                if (key.Key == ConsoleKey.S) CountersForm.CreateAndShow(controller);
                if (key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Alt) if (Updates.DownloadInstallerAndRun(config.GetBool("updates.includePrerelease", false)).Result) break;
                if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Alt) controller.AutoDetectOffsets(config);
                if (key.Key == ConsoleKey.B && key.Modifiers == ConsoleModifiers.Control) Clipboard.CopyProxyBypassToClipboard();
                if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Alt) controller.ManualFFRKRestart();

            }
            
            // Stop
            controller.Stop();
            OnConsoleExit();

        }

        private static void OnConsoleExit()
        {
            // Kill adb option
            if (new ConfigHelper().GetBool("adb.closeOnExit", false)) Adb.KillAdb();

            // Flush log file buffer
            ColorConsole.LogBuffer.Flush();
        }

    }
}
