using FFRK_Machines;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    class LabStateJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var enabled = dataMap.GetBoolean("enabled");
            var controller = (LabController)dataMap["controller"];

            if (enabled)
            {
                var autoStart = controller.Machine.Config.AutoStart;
                controller.Machine.Config.AutoStart = true;
                ColorConsole.WriteLine(ConsoleColor.Green, "Enabling due to schedule: {0}", context.Trigger.Description);
                controller.Enable();
                controller.Machine.Config.AutoStart = autoStart;
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Disabling due to schedule: {0}", context.Trigger.Description);
                controller.Machine.DisableSafe();
            }
            return Task.CompletedTask;
        }
    }
}
