using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionCode = string.Format("v{0}.{1}.{2}-beta", version.Major, version.Minor, version.Build);
            ColorConsole.WriteLine("{0} {1}", Assembly.GetExecutingAssembly().GetName().Name, versionCode);
            if (config.GetBool("updates.checkForUpdates", false)) Updates.Check("hughjeffner", "ffrk-labmem", true, versionCode);
           
            // Controller
            LabController controller = new LabController(
                adbPath: config["adb.path"],
                adbHost: config["adb.host"],
                proxyPort: config.GetInt("proxy.port", 8081),
                configFile: config["lab.configFile"],
                topOffset: config.GetInt("screen.topOffset", 0),
                bottomOffset: config.GetInt("screen.bottomOffset", 0)
            );

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
            }
            
            // Stop
            controller.Stop();

        }

    }
}
