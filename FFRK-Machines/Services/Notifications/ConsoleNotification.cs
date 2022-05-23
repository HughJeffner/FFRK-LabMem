using System;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Notifications
{
    public class ConsoleNotification : Notifications.Notification
    {

        public async override Task Notify(Notifications.NotificationArgs args)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation, "Playing console beeps");
            Task mytask = Task.Run(async () =>
            {
                if (OperatingSystem.IsWindows())
                {
                    Console.Beep(523, 200);
                    Console.Beep(523, 200);
                    Console.Beep(523, 200);
                    Console.Beep(523, 500);
                    Console.Beep(415, 500);
                    Console.Beep(466, 500);
                    Console.Beep(523, 300);
                    Console.Beep(466, 250);
                    Console.Beep(523, 800);
                } else
                {
                    Console.Beep();
                    await Task.Delay(50);
                    Console.Beep();
                    await Task.Delay(50);
                    Console.Beep();
                    await Task.Delay(50);
                    Console.Beep();
                    await Task.Delay(125); 
                    Console.Beep();
                    await Task.Delay(125);
                    Console.Beep();
                    await Task.Delay(125);
                    Console.Beep();
                    await Task.Delay(75);
                    Console.Beep();
                    await Task.Delay(66);
                    Console.Beep();
                  }
            });

            await mytask;

        }
    }
}
