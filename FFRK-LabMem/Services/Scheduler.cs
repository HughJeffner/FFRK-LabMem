using FFRK_LabMem.Machines;
using FFRK_Machines;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Services
{
    public class Scheduler
    {
        private const string CONFIG_PATH = "./Config/schedules.json";

        private static Scheduler _instance = null;

        StdSchedulerFactory factory = new StdSchedulerFactory();
        IScheduler scheduler;
        IJobDetail job;

        public List<Schedule> Schedules { get; set; }

        private Scheduler(LabController controller)
        {
            _ = Initalize(controller);
        }

        public static Scheduler Default(LabController controller)
        {
            if (_instance == null) _instance = new Scheduler(controller);
            return _instance;
        }

        private async Task Initalize(LabController controller)
        {
            // get a scheduler
            scheduler = await factory.GetScheduler();

            // define the jobs
            job = JobBuilder.Create<LabStateJob>()
                .WithIdentity("enableJob", "group1")
                .WithDescription("Enables the bot")
                .StoreDurably(true)
                .Build();
            job.JobDataMap.Put("controller", controller);

            // Load from disk
            await Load();

        }

        public async Task Start()
        {

            await scheduler.AddJob(job, true);

            foreach(var schedule in Schedules)
            {

                // Only if in the future or schedule present
                if (schedule.EnableEnabled && (schedule.EnableDate > DateTime.Now || !String.IsNullOrEmpty(schedule.EnableCronTab)))
                {

                    // Build the trigger
                    var builder = TriggerBuilder.Create()
                        .WithIdentity(schedule.Name + "_enable")
                        .ForJob(job)
                        .StartAt(schedule.EnableDate)
                        .UsingJobData("enabled", true)
                        .UsingJobData("hardstart", schedule.EnableHardStart)
                        .WithDescription(schedule.Name);

                    // Repeat
                    if (!String.IsNullOrEmpty(schedule.EnableCronTab))
                    {
                        builder.WithCronSchedule(schedule.EnableCronTab);
                    }

                    // Schedule the job
                    await scheduler.ScheduleJob(builder.Build());
                }

                if (schedule.DisableEnabled && (schedule.DisableDate > DateTime.Now || !String.IsNullOrEmpty(schedule.DisableCronTab)))
                {
                    // Build the trigger
                    var builder = TriggerBuilder.Create()
                        .WithIdentity(schedule.Name + "_disable")
                        .ForJob(job)
                        .StartAt(schedule.DisableDate)
                        .UsingJobData("enabled", false)
                        .UsingJobData("closeapp", schedule.DisableCloseApp)
                        .WithDescription(schedule.Name);

                    // Repeat
                    if (!String.IsNullOrEmpty(schedule.DisableCronTab))
                    {
                        builder.WithCronSchedule(schedule.DisableCronTab);
                    }

                    // Schedule the job
                    await scheduler.ScheduleJob(builder.Build());
                }

            }

            // Start the service
            await scheduler.Start();
            if (Schedules.Count > 0)
            {
                var triggs = await scheduler.GetTriggersOfJob(job.Key);
                var next = triggs.Min(t => t.GetNextFireTimeUtc());
                if (next.HasValue)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkGreen, "{0} Schedule(s) loaded, next: {1}", Schedules.Count, next.Value.ToLocalTime());
                }
                
            }
        }

        public async Task Stop()
        {
            await scheduler.Clear();
            await scheduler.Standby();
        }

        public async Task Load()
        {
            try
            {
                this.Schedules = JsonConvert.DeserializeObject<List<Schedule>>(File.ReadAllText(CONFIG_PATH));
            } catch (Exception)
            {
                this.Schedules = new List<Schedule>();
            }
            await Task.CompletedTask;
        }

        public async Task Save()
        {
            File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this.Schedules, Formatting.Indented));
            await Task.CompletedTask;
        }

        public class Schedule
        {
            public string Name { get; set; }
            public bool EnableEnabled { get; set; }
            public DateTime EnableDate { get; set; }
            public String EnableCronTab { get; set; }
            public Boolean EnableHardStart { get; set; }
            public bool DisableEnabled { get; set; }
            public DateTime DisableDate { get; set; }
            public String DisableCronTab { get; set; }
            public Boolean DisableCloseApp { get; set; }
        }

    }
}
