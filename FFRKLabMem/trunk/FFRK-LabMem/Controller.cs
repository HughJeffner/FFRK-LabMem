using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;

namespace FFRK_LabMem
{
    class Controller
    {

        public Lab Lab { get; set; }
        public Proxy Proxy { get; set; }
        public Adb Adb { get; set; }

        public Controller(Lab lab, Proxy proxy, Adb adb)
        {
            this.Lab = lab;
            this.Proxy = proxy;
            this.Adb = adb;
            this.Proxy.OnPaintingsLoaded += Proxy_OnPaintingsLoaded;

        }

        void Proxy_OnPaintingsLoaded(Newtonsoft.Json.Linq.JArray paintings)
        {
            Lab.OnPaintingsLoaded(paintings);
        }
        
    }
}
