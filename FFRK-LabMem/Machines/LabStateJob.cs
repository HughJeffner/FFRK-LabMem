﻿using FFRK_Machines.Services;
using FFRK_Machines;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    [DisallowConcurrentExecution]
    class LabStateJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.MergedJobDataMap;
            var enabled = dataMap.GetBoolean("enabled");
            var controller = (LabController)dataMap["controller"];


            try
            {
                if (enabled)
                {
                    if (controller.Enabled)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "LabMem already running for schedule: {0}", context.Trigger.Description);
                        return;
                    }
                    if (dataMap.GetBoolean("hardstart"))
                    {
                        ColorConsole.WriteLine(ConsoleColor.Green, "Restarting FFRK due to schedule: {0}", context.Trigger.Description);
                        controller.Enable();
                        await controller.Machine.ManualFFRKRestart(false);
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Green, "Enabling due to schedule: {0}", context.Trigger.Description);
                        await EnableWithAutoStart(controller, context);
                    }

                } else
                {
                    if (!controller.Enabled)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "LabMem already stopped for schedule: {0}", context.Trigger.Description);
                        return;
                    }
                    if (dataMap.GetBoolean("closeapp"))
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, "Closing FFRK due to schedule: {0}", context.Trigger.Description);
                        controller.Disable();
                        await controller.Adb.StopPackage(Adb.FFRK_PACKAGE_NAME, CancellationToken.None);
                    } else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, "Disable requested due to schedule: {0}", context.Trigger.Description);
                        controller.Machine.DisableSafe();
                    }
                
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
            }
            
            await Task.CompletedTask;
        }

        private async Task EnableWithAutoStart(LabController controller, IJobExecutionContext context)
        {   
            var autoStart = controller.Machine.Config.AutoStart;
            controller.Machine.Config.AutoStart = true;
            controller.Enable();
            await Task.Delay(1000);
            controller.Machine.Config.AutoStart = autoStart;
        }
    }
}
