using System;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Notifications
{
    class ConsoleNotification : Notifications.Notification
    {

        public async override Task Notify()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation, "Playing console beeps");
            Console.Beep(523, 200);
            Console.Beep(523, 200);
            Console.Beep(523, 200);
            Console.Beep(523, 500);
            Console.Beep(415, 500);
            Console.Beep(466, 500);
            Console.Beep(523, 300);
            Console.Beep(466, 250);
            Console.Beep(523, 800);
            await Task.FromResult(0);

        }
    }
}
