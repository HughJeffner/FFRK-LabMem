﻿using Newtonsoft.Json;
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
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
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
            { EventType.LAB_COMPLETED, new List<Notification>(){
                new SoundNotification(){ FilePath = Sound.FF1_Victory}
            }},
            { EventType.LAB_FAULT, new List<Notification>(){
                new SoundNotification(){ FilePath = Sound.FF1_Inn}
            }}
        };

        private Notifications()
        {
            
        }

        public static Notifications Default
        {
            get
            {
                if (_instance == null) _instance = new Notifications();
                return _instance;
            }
        }

        public static async Task Initalize()
        {
            await Sound.Initalize();
            await Default.Load();
        }

        public async Task ProcessEvent(EventType eventType, string message) {

            await ProcessEvent(eventType, message, this.Events);
            
        }

        public async Task ProcessEvent(EventType eventType, string message, EventList events)
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation, "Processing event {0}", eventType);
            try
            {
                var args = new NotificationArgs() { EventType = eventType, Message = message };
                await Task.WhenAll(events[eventType].Where(n => n.Enabled).Select(t => t.Notify(args)));

                //foreach (var item in this.Events[eventType].Where(i => i.Enabled))
                //{
                //    if (item.Enabled) await item.Notify();
                //}
            }
            catch (Exception e)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, e.ToString());
            }

        }

        public static Task<EventList> GetEvents(string path = CONFIG_PATH)
        {
            try
            {
                return Task.FromResult(JsonConvert.DeserializeObject<EventList>(File.ReadAllText(path), jsonSerializerSettings));
            }
            catch (FileNotFoundException) { }
            catch (Exception e)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, e.ToString());
            }
            return Task.FromResult(Default.Events);
        }

        public async Task Load()
        {
            this.Events = await GetEvents();
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

        public class NotificationArgs
        {
            public EventType EventType { get; set; }
            public String Message { get; set; }
        }

        public abstract class Notification
        {
            public bool Enabled { get; set; } = true;
            public abstract Task Notify(NotificationArgs args);
        }

    }
}
