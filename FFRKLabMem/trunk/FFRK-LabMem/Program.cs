using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;

namespace FFRK_LabMem
{
    class Program
    {

        static void Main(string[] args)
        {

            // Controller & State machine
            Controller controller = null;
            Lab lab = null;

            // Proxy Server
            Proxy proxy = new Proxy();
            proxy.Start();

            // Adb
            Adb adb = new Adb();
            if (adb.Connect())
            {
                lab = new Lab(adb.Device);
                controller = new Controller(lab, proxy);
            }         
                       
            // Ad-hoc command loop
            Console.WriteLine("Press 'E' to Exit");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E) break;
            }
            
            // Stop proxy
            proxy.Stop();

        }

    }
}
