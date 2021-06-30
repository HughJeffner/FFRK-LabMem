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

        public Controller(Proxy proxy, Adb adb)
        {
            this.Lab = new Lab(adb);
            this.Proxy = proxy;
            this.Adb = adb;
            this.Lab.RegisterWithProxy(proxy);
        }

    }
}
