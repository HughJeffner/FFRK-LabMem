using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;

namespace FFRK_LabMem
{
    class Controller
    {

        public Lab Lab { get; set; }
        public Proxy Proxy { get; set; }
        public Adb Adb { get; set; }

        public Controller(String adbPath, String adbHost, int proxyPort, Lab.LabPriorityStrategy priorityStrategy)
        {

            // Proxy Server
            this.Proxy = new Proxy(proxyPort);
            this.Proxy.Start();

            // Adb
            this.Adb = new Adb(adbPath, adbHost);

            // Start if connected
            if (this.Adb.Connect())
            {
                this.Lab = new Lab(this.Adb, priorityStrategy);
                this.Lab.RegisterWithProxy(this.Proxy);
            }

        }

        public void Stop()
        {
            Proxy.Registrations.Clear();
            this.Lab = null;
            ColorConsole.WriteLine(ConsoleColor.Red, "Disabled");
        }

    }
}
