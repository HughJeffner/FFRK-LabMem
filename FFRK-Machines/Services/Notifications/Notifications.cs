using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Notifications
{
    public class Notifications
    {

        private const string CONFIG_PATH = "./Config/notifications.json";
        private static Notifications _instance = null;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        public enum EventType
        {
            LAB_COMPLETED,
            LAB_FAULT
        }

        public EventList Events { get; set; } = new EventList {
            { EventType.LAB_COMPLETED, new List<Notification>(){new SoundNotification(){ FilePath = Sound.FF1_Victory}}},
            { EventType.LAB_FAULT, new List<Notification>(){new SoundNotification(){ FilePath = Sound.FF1_Inn}}}
        };

        private Notifications()
        {
            _ = Initalize();
        }

        public static Notifications Default
        {
            get
            {
                if (_instance == null) _instance = new Notifications();
                return _instance;
            }
        }

        private async Task Initalize()
        {
            await Task.CompletedTask;
        }

        public async Task ProcessEvent(EventType eventType) {

            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation,"Processing event {0}", eventType);
            try
            {
                foreach (var item in this.Events[eventType])
                {
                    await item.Notify();
                }
            } catch (Exception e)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, e.ToString());
            }
            
        }

        public async Task Load()
        {
            try
            {
                this.Events = JsonConvert.DeserializeObject<EventList>(File.ReadAllText(CONFIG_PATH), jsonSerializerSettings);
            }
            catch (Exception)
            {
                this.Events = new EventList();
            }
            await Task.CompletedTask;
        }

        public async Task Save()
        {
            
            File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this.Events, jsonSerializerSettings));
            await Task.CompletedTask;
        }

        public class EventList : Dictionary<EventType, List<Notification>>
        {
        }

        public abstract class Notification
        {
            public abstract Task Notify();
        }

    }
}
