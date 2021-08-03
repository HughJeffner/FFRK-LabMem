using System;
using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;

namespace FFRK_LabMem
{
    class Program
    {

        static void Main(string[] args)
        {

            // Get Configuration
            var configFile = (args.Length > 0) ? args[0] : null;
            var config = new ConfigHelper(configFile);

            // Console
            ColorConsole.Timestamps = config.GetBool("console.timestamps", true);

            // Version check
            var versionCode = Updates.GetVersionCode("beta");
            ColorConsole.WriteLine("{0} {1}", Updates.GetName(), versionCode);
            if (config.GetBool("updates.checkForUpdates", false)) Updates.Check("hughjeffner", "ffrk-labmem", true, versionCode);
           
            // Controller
            LabController controller = LabController.CreateAndStart(config).Result;

            // Ad-hoc command loop
            Console.WriteLine("Press 'D' to Disable, 'E' to Enable, 'X' to Exit");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.X) break;
                if (key.Key == ConsoleKey.E) controller.Enable();
                if (key.Key == ConsoleKey.D) controller.Disable();
                if (key.Key == ConsoleKey.H) Tray.MinimizeTo();
                if (key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Alt) Updates.OpenReleasesInBrowser("hughjeffner", "ffrk-labmem");
                if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Alt) controller.AutoDetectOffsets(config);
            }
            
            // Stop
            controller.Stop();

        }

    }
}
