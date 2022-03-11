using FFRK_LabMem.Machines;
using FFRK_Machines.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Services
{
    class Tyro
    {
        public static void Check(LabController controller)
        {
            if (!controller.Enabled) return;
            var now = DateTime.UtcNow;
            if (new DateTime(2022, 3, 25) <= now && now <= new DateTime(2022, 3, 27))
            {
                Console.WriteLine(Properties.Resources.Tyro7);
                Sound.Play(Sound.FF1_Event);
            }
        }
    }
}
