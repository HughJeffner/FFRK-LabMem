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
                priorityStrategy: config.GetEnum<FFRK_LabMem.Machines.Lab.LabPriorityStrategy>("lab.priorityStrategy")
            );

            // Ad-hoc command loop
            Console.WriteLine("Press 'E' to Exit");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E) break;
                if (key.Key == ConsoleKey.T) controller.Stop();
            }
            
            // Stop
            controller.Stop();

        }

    }
}
