using System;
using FFRK_LabMem.Config;
using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Data.UI;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using FFRK_Machines;

namespace FFRK_LabMem
{
    class Program
    {
        static void Main(string[] args)
        {

            // Listen for console exit
            ConsoleExit.Listen(OnConsoleExit);

            // Get Configuration
            var configFile = (args.Length > 0) ? args[0] : null;
            var config = new ConfigHelper(configFile);

            // Console
            ColorConsole.Timestamps = config.GetBool("console.timestamps", true);
            ColorConsole.DebugCategories = (ColorConsole.DebugCategory)config.GetInt("console.debugCategories", 0);
            
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
            Console.WriteLine("Press 'D' to Disable, 'E' to Enable, 'C' for Config, 'Ctrl+X' to Exit");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control) break;
                    if (key.Key == ConsoleKey.E) controller.Enable();
                    if (key.Key == ConsoleKey.D) controller.Disable();
                    if (key.Key == ConsoleKey.H) Tray.MinimizeTo(key.Modifiers);
                    if (key.Key == ConsoleKey.C) ConfigForm.CreateAndShow(config, controller);
                    if (key.Key == ConsoleKey.S) CountersForm.CreateAndShow(controller);
                    if (key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Alt) Updates.DownloadInstallerAndRun(config.GetBool("updates.includePrerelease", false));
                    if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Alt) controller.AutoDetectOffsets(config);
                    if (key.Key == ConsoleKey.B && key.Modifiers == ConsoleModifiers.Control) Clipboard.CopyProxyBypassToClipboard();
                    if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Alt) controller.ManualFFRKRestart();

                }
                // Needed to run winforms
                System.Windows.Forms.Application.DoEvents();
            }
            
            // Stop
            controller.Stop();
            OnConsoleExit();

        }

        private static void OnConsoleExit()
        {
            // Kill adb option
            if (new ConfigHelper().GetBool("adb.closeOnExit", false)) Adb.KillAdb();
        }

    }
}
