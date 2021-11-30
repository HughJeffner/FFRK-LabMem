using System.Threading.Tasks;

namespace FFRK_Machines.Services.Notifications
{
    public class SoundNotification : Notifications.Notification
    {

        public string FilePath { get; set; }

        public async override Task Notify()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation, "Playing sound: {0}", FilePath);
            Sound.Play(FilePath);
            await Task.CompletedTask;
        }
    }
}
