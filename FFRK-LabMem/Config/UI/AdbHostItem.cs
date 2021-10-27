using System;

namespace FFRK_LabMem.Config.UI
{
    public class AdbHostItem
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public String Display
        {
            get
            {
                return String.Format("{0} [{1}]", Value, Name);
            }
        }
    }
}
