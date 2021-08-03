namespace FFRK_Machines.Machines
{
    /// <summary>
    /// Base machine configuration class
    /// </summary>
    public class MachineConfiguration
    {

        public bool Debug { get; set; }

        public MachineConfiguration()
        {
            Debug = true;
        }

    }
}
