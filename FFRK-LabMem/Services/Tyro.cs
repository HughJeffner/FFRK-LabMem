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
        private static bool check7 = false;
        private static readonly List<ConsoleKey> keyBuffer = new List<ConsoleKey>();
        private static bool Check7(LabController controller)
        {
            if (controller.Enabled)
            {
                var now = DateTime.UtcNow;
                var target = new DateTime(2022, 3, 25);
                if (target <= now && now <= target.AddDays(2) && !check7)
                {
                    Console.WriteLine(Properties.Resources.Tyro7);
                    Sound.Play(Sound.FF1_Event);
                    check7 = true;
                    return true;
                }
            }
            return false;
        }
        private static void CheckBuffer()
        {
            var bufferString = String.Concat(keyBuffer);
            if (bufferString.Equals("EPK")) Console.WriteLine(Properties.Resources.EPK);
        }

        public static void Register(LabController controller)
        {
            if (!Check7(controller))
                controller.OnEnabled += (s, e) => Tyro.Check7(controller);
        }

        public static void ReadConsole(ConsoleKeyInfo key)
        {
            keyBuffer.Add(key.Key);
            if (keyBuffer.Count > 3) keyBuffer.RemoveAt(0);
            CheckBuffer();
        }

    }
}
