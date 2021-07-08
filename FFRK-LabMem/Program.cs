using System;
using System.Collections.Generic;
using System.Linq;
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

            // Controller
            Controller controller = new Controller(
                adbPath: config["adb.path"],
                adbHost: config["adb.host"],
                proxyPort: config.GetInt("proxy.port"),
                priorityStrategy: config.GetEnum<FFRK_LabMem.Machines.Lab.LabPriorityStrategy>("lab.priorityStrategy"),
                debug: config.GetBool("console.debug")
            );

            // Ad-hoc command loop
            Console.WriteLine("Press 'X' to Exit");
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
