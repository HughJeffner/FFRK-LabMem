using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Machines
{
    /// <summary>
    /// Base machine configuration class
    /// </summary>
    public class MachineConfiguration
    {
       
        public bool Debug { get; set; }

        public MachineConfiguration()
        {
            this.Debug = true;
        }

    }
}
