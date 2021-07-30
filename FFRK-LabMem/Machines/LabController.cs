using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;


namespace FFRK_LabMem.Machines
{
    class LabController : MachineController<Lab, Lab.State, Lab.Trigger, Lab.Configuration>
    {

        public LabController(bool debug, String adbPath, String adbHost, int proxyPort, int topOffset, int bottomOffset, String configFile)
        {
            base.Start(debug, adbPath, adbHost, proxyPort, topOffset, bottomOffset, configFile, Lab.State.Unknown);
        }

        protected override Lab CreateMachine(Lab.Configuration config)
        {
            return new Lab(this.Adb, config);
        }

    }
}
