using System;
using System.Runtime.InteropServices;

namespace FFRK_LabMem.Services
{
    class ConsoleTasks
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const uint ENABLE_MOUSE_INPUT = 0x0010;
        const uint ENABLE_PROCESSED_INPUT = 0x0001;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        private static EventHandler handler;   // Keeps it from getting garbage collected
        private delegate bool ConsoleEventDelegate(int eventType);
        public static event Action OnConsoleExit;

        public static void ListenForExit(EventHandler eventHandler)
        {
            handler = eventHandler;
            AppDomain.CurrentDomain.ProcessExit += handler;
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                OnConsoleExit();
            }
            return false;
        }

        public static bool DisableQuickEditMode()
        {

            if (!OperatingSystem.IsWindows()) return false;

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // Remove mouse input flag and add processed input flag (scroll wheel fix)
            consoleMode &= ~ENABLE_MOUSE_INPUT;
            consoleMode |= ENABLE_PROCESSED_INPUT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }

    }
}
