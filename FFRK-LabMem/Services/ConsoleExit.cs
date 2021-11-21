using System;
using System.Runtime.InteropServices;

namespace FFRK_LabMem.Services
{
    class ConsoleExit
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        private static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        private delegate bool ConsoleEventDelegate(int eventType);
        public static event Action OnConsoleExit;

        public static void Listen(Action action)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            OnConsoleExit = action;
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                OnConsoleExit();
            }
            return false;
        }

    }
}
