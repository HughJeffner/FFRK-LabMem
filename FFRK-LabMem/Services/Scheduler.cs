using FFRK_LabMem.Machines;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Services
{
    class Scheduler
    {
        StdSchedulerFactory factory = new StdSchedulerFactory();
        IScheduler scheduler;
        IJobDetail enableJob;
        IJobDetail disableJob;

        public Scheduler(LabController controller)
        {

            _ = Initalize(controller);
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

        }

        public async Task Start(List<LabConfiguration.Schedule> schedules)
        {

            foreach(var schedule in schedules)
            {
                // Trigger the job to run now, and then every 40 seconds
                var builder = TriggerBuilder.Create()
                    .WithDescription(schedule.Name)
                    .StartAt(schedule.StartDate);

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

               ITrigger trigger = builder.Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob((schedule.Enable)?enableJob:disableJob, trigger);
            }
           
            await scheduler.Start();
        }

        public async Task Stop()
        {
            await scheduler.Standby();
        }

    }
}
