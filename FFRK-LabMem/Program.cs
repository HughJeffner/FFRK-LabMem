using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Config;
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
            ColorConsole.Timestamps = config.GetBool("console.timestamps");

            // Version check
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionCode = string.Format("v{0}.{1}.{2}-beta", version.Major, version.Minor, version.Build);
            ColorConsole.WriteLine("{0} {1}", Assembly.GetExecutingAssembly().GetName().Name, versionCode);
            if (config.GetBool("updates.checkForUpdates")) UpdateChecker.Check("hughjeffner", "ffrk-labmem", versionCode);
           
            // Controller
            LabController controller = new LabController(
                adbPath: config["adb.path"],
                adbHost: config["adb.host"],
                proxyPort: config.GetInt("proxy.port"),
                configFile: config["lab.configFile"],
                topOffset: config.GetInt("screen.topOffset"),
                bottomOffset: config.GetInt("screen.bottomOffset")
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
            }
            
            // Stop
            controller.Stop();

        }

    }
}
