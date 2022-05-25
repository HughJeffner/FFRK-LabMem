using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Config
{
    public interface IAppConfigSection
    {
        public void Load(ConfigHelper helper);
        public void Save();

    }
}
