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

        public Controller()
        {

            // Proxy Server
            this.Proxy = new Proxy();
            this.Proxy.Start();

            // Adb
            this.Adb = new Adb();

            // Start if connected
            if (this.Adb.Connect())
            {
                this.Lab = new Lab(this.Adb);
                this.Lab.RegisterWithProxy(this.Proxy);
            }         

        }

        public void Stop()
        {
            Proxy.Stop();
        }

    }
}
