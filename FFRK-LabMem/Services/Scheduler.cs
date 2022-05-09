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
        readonly StdSchedulerFactory factory = new StdSchedulerFactory();
        IScheduler scheduler;
        IJobDetail job;

        public List<Schedule> Schedules { get; set; }
        public int MaintenanceDoneHourUtc { get; set; } = 13;

        private Scheduler(LabController controller)
        {
            _ = Initalize(controller);
        }

        public static async Task Init(LabController controller)
        {
            _instance = new Scheduler(controller);
            await _instance.Start();
        }

        public static Scheduler Default
        {
            get
            {
                if (_instance == null) throw new InvalidOperationException("Please call Init before using the default instance property");
                return _instance;
            }
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

                // Data Map
                JobDataMap jobDataMap = new JobDataMap();
                jobDataMap.Add("schedule", schedule);

                // Only if in the future or schedule present
                if (schedule.EnableEnabled && (schedule.EnableDate > DateTime.Now || !String.IsNullOrEmpty(schedule.EnableCronTab)))
                {

                    // Build the trigger
                    var builder = TriggerBuilder.Create()
                        .WithIdentity(schedule.Name + "_enable")
                        .ForJob(job)
                        .StartAt(schedule.EnableDate)
                        .UsingJobData(jobDataMap)
                        .UsingJobData("enabled", true)
                        .WithDescription(schedule.Name);

                    // Repeat
                    if (!String.IsNullOrEmpty(schedule.EnableCronTab))
                    {
                        builder.WithCronSchedule(schedule.EnableCronTab, b => b.WithMisfireHandlingInstructionDoNothing());
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
                        .UsingJobData(jobDataMap)
                        .UsingJobData("enabled", false)
                        .WithDescription(schedule.Name);

                    // Repeat
                    if (!String.IsNullOrEmpty(schedule.DisableCronTab))
                    {
                        builder.WithCronSchedule(schedule.DisableCronTab, b => b.WithMisfireHandlingInstructionDoNothing());
                    }

                    // Schedule the job
                    await scheduler.ScheduleJob(builder.Build());
                }

            }

            // Start the service
            await scheduler.Start();

            // Show status
            if (Schedules.Count > 0)
            {
                // Need a delay before getting the next trigger time so any misfires can be handled and triggers get updated accordingly
                await Task.Delay(50).ConfigureAwait(false);

                // Get all triggers for our job and filter out the next one
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
            // Stop temporarily while we save
            await Stop();

            // Persist to disk
            File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this.Schedules, Formatting.Indented));

            // Re-start
            await Start();
        }

        public async Task AddPostMaintenanceSchedule()
        {

            // Return if option invalid or disabled
            if (this.MaintenanceDoneHourUtc < 0 || this.MaintenanceDoneHourUtc > 23) return;

            var nowUtc = DateTime.UtcNow;
            var maintenanceDoneUtc = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, this.MaintenanceDoneHourUtc, 0, 0, 0, DateTimeKind.Utc);
            var name = $"Post-Maintenance ({nowUtc.ToShortDateString()})";

            // Return if in the past or already added
            if (maintenanceDoneUtc < nowUtc || this.Schedules.Any(s => s.Name.Equals(name))) return;

            this.Schedules.Add(new Schedule()
            {
                Name = name,
                EnableDate = maintenanceDoneUtc.ToLocalTime(),
                EnableHardStart = true,
                EnableEnabled = true,
                DisableDate = DateTime.Now
            });

            await Save();

        }

        public class Schedule
        {
            public string Name { get; set; }
            public bool EnableEnabled { get; set; }
            public DateTime EnableDate { get; set; }
            public String EnableCronTab { get; set; }
            public Boolean EnableHardStart { get; set; }
            public Boolean EnableForceStart { get; set; }
            public bool DisableEnabled { get; set; }
            public DateTime DisableDate { get; set; }
            public String DisableCronTab { get; set; }
            public Boolean DisableCloseApp { get; set; }
        }

    }
}
