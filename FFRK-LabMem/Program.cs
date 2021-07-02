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

            // Controller
            Controller controller = new Controller();

            // Ad-hoc command loop
            Console.WriteLine("Press 'E' to Exit");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E) break;
                if (key.Key == ConsoleKey.D) controller.Disable();
            }
            
            // Stop
            controller.Stop();

        }

    }
}
