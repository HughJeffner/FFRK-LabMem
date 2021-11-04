using FFRK_LabMem.Machines;
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
        IJobDetail enableJob;
        IJobDetail disableJob;

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
            enableJob = JobBuilder.Create<LabStateJob>()
                .WithIdentity("enableJob", "group1")
                .WithDescription("Enables the bot")
                .UsingJobData("enabled", true)
                .Build();
            enableJob.JobDataMap.Put("controller", controller);

            disableJob = JobBuilder.Create<LabStateJob>()
                .WithIdentity("disableJob", "group1")
                .WithDescription("Disables the bot")
                .UsingJobData("enabled", false)
                .Build();
            disableJob.JobDataMap.Put("controller", controller);

            // Load from disk
            await Load();

        }

        public async Task Start()
        {

            foreach(var schedule in Schedules)
            {

                if (!schedule.Enabled) continue;

                // Build the trigger
                var builder = TriggerBuilder.Create()
                    .WithDescription(schedule.Name)
                    .StartAt(schedule.StartDate);

                // Optional end time
                if (schedule.EndDate != DateTime.MaxValue) builder.EndAt(schedule.EndDate);

                // Crontab or simple schedule
                if (!String.IsNullOrEmpty(schedule.CronTab))
                {
                    builder.WithCronSchedule(schedule.CronTab);
                } else
                {
                    builder.WithSimpleSchedule(x => x
                        .WithIntervalInHours(24)
                        .RepeatForever()
                    );
                }

                // Schedule the job
                await scheduler.ScheduleJob((schedule.Enabled)?enableJob:disableJob, builder.Build());
            }
           
            await scheduler.Start();
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
            public bool Enabled { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; } = DateTime.MaxValue;
            public String CronTab { get; set; }
        }

    }
}
