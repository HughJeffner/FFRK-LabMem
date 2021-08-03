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
