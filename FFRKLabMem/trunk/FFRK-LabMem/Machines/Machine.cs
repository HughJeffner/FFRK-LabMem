using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;

namespace FFRK_LabMem.Machines
{
    public abstract class Machine
    {
        /// <summary>
        /// Gives a chance for this machine to register with the proxy
        /// </summary>
        /// <param name="Proxy"></param>
        public abstract void RegisterWithProxy(Proxy Proxy);

        /// <summary>
        /// Data mached from registrations is passed from the proxy to the machine
        /// </summary>
        /// <param name="UrlContains"></param>
        /// <param name="data"></param>
        public abstract Task PassFromProxy(String UrlContained, JObject data);
    }
}
