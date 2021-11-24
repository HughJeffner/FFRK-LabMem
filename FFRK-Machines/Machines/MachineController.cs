using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json.Linq;

namespace FFRK_Machines.Machines
{
    /// <summary>
    /// An abstract class that controls a machine
    /// </summary>
    /// <typeparam name="M">The machine class</typeparam>
    /// <typeparam name="S">An enum of all the possible machine states</typeparam>
    /// <typeparam name="T">An enum of all the available machine triggers</typeparam>
    /// <typeparam name="C">A class containing configuration information for the machine, must inherit from MachineConfiguration</typeparam>
    public abstract class MachineController<M, S, T, C>
        where M : Machine<S, T, C>
        where C : MachineConfiguration
    {

        public event EventHandler OnEnabled;
        public event EventHandler OnDisabled;

        private bool enabled = false;
        private bool proxySecure = false;
        private bool proxyAutoConfig = false;
        public M Machine { get; set; }
        public Proxy Proxy { get; set; }
        public Adb Adb { get; set; }
        public bool Enabled => enabled;
        private BlockingCollection<Proxy.ProxyEventArgs> queue = new BlockingCollection<Proxy.ProxyEventArgs>();

        /// <summary>
        /// Implementors create an instance of the machine you are controlling
        /// </summary>
        /// <param name="config">The configuration needed to create the machine</param>
        /// <returns></returns>
        protected abstract M CreateMachine(C config);

        /// <summary>
        /// Starts the machine controller
        /// </summary>
        /// <param name="adbPath">Path to adb executable</param>
        /// <param name="adbHost">TCP host for adb connection</param>
        /// <param name="proxyPort">Port to listen for http proxy requests</param>
        /// <param name="proxySecure">Intercept and decrypt https requests</param>
        /// <param name="proxyBlocklist">Path to proxy blocklist file</param>
        /// <param name="proxyBlocklist">Automatic configure of system proxy settings</param>
        /// <param name="topOffset">Top offset of screen</param>
        /// <param name="bottomOffset">Bottom offest of screen</param>
        /// <param name="configFile">Path to the machine config file</param>
        /// <param name="unkownState">State the machine should enter if reset, or unknown state</param>
        public async Task Start(string adbPath, string adbHost, int proxyPort, bool proxySecure, string proxyBlocklist, bool proxyAutoConfig, bool proxyConnectionPooling, int topOffset, int bottomOffset, string configFile, int consumers = 2)
        {

            // Adb
            this.Adb = new Adb(adbPath, adbHost, topOffset, bottomOffset);
            this.Adb.DeviceUnavailable += Adb_DeviceUnavailable;

            // Proxy Server
            Proxy = new Proxy(proxyPort, proxySecure, proxyBlocklist, proxyConnectionPooling);
            this.Proxy.ProxyEvent += Proxy_ProxyEvent;
            this.proxySecure = proxySecure;
            this.proxyAutoConfig = proxyAutoConfig;
            Proxy.Start();

            // Machine
            ColorConsole.WriteLine("Setting up {0} with config: {1}", typeof(M).Name, configFile);
            Machine = this.CreateMachine(await MachineConfiguration.Load<C>(configFile));
            Machine.MachineFinished += Machine_MachineFinished;
            Machine.MachineError += Machine_MachineError;

            // Start if connected
            if (await Adb.Connect())
            {
                // Hook up events
                await EngageMachine();
                this.Enable();

            } else
            {
                // Wait for device
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Waiting for a device to become available");
                this.Adb.DeviceAvailable += Adb_DeviceAvailable;
                this.enabled = false;
            }

            // Start consumers
            for (int i = 0; i < consumers; i++)
            {
                var consumer = Task.Factory.StartNew(() => Consume());
            }

        }

        private async void Consume()
        {
            Thread.CurrentThread.Name = "Machine Worker";
            foreach (var item in queue.GetConsumingEnumerable())
            {
                try
                {
                    var data = JObject.Parse(item.Body.Substring(1));
                    await item.Registration.Handler(data, item.Registration.UrlMatch.ToString());
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                }

            }

        }

        private async Task EngageMachine()
        {
            Machine.RegisterWithProxy(Proxy);
            if (this.proxySecure) await Adb.InstallRootCert("rootCert.pfx", CancellationToken.None);
            if (this.proxyAutoConfig) await Adb.SetProxySettings(this.Proxy.Port, CancellationToken.None);
        }

        private void Adb_DeviceAvailable(object sender, SharpAdbClient.DeviceDataEventArgs e)
        {
            if (Adb.Connect().Result)
            {
                _ = EngageMachine();
                this.Adb.DeviceAvailable -= Adb_DeviceAvailable;
                this.Enable();
            }
        }

        private void Adb_DeviceUnavailable(object sender, SharpAdbClient.DeviceDataEventArgs e)
        {
            this.Disable();
        }

        // Event handlers
        void Machine_MachineError(object sender, Exception e)
        {
            ColorConsole.WriteLine(ConsoleColor.Red, "Something unexpected happened ({0}).  To prevent damages, {1} will now be reset.  Please re-enable when ready with 'E'", e.Message, typeof(M).Name);
            Disable();
        }

        void Machine_MachineFinished(object sender, EventArgs e)
        {
            Disable();
        }

        void Proxy_ProxyEvent(object sender, Proxy.ProxyEventArgs e)
        {
            if (enabled) queue.Add(e);
        }

        /// <summary>
        /// Enables the machine
        /// </summary>
        public void Enable()
        {
            if (!enabled && Machine != null && this.Adb.HasDevice)
            {
                enabled = true;
                this.Machine.Enable();
                ColorConsole.WriteLine(ConsoleColor.Green, "Enabled {0}", typeof(M).Name);
                if (OnEnabled != null) OnEnabled.Invoke(this, new EventArgs());
            }

        }

        /// <summary>
        /// Disables the machine
        /// </summary>
        public void Disable()
        {

            if (enabled && Machine != null)
            {
                enabled = false;
                this.Machine.Disable();
                ColorConsole.WriteLine(ConsoleColor.Red, "Disabled {0}", typeof(M).Name);
                if (OnDisabled != null) OnDisabled.Invoke(this, new EventArgs());
            }

        }

        /// <summary>
        /// Stops the machine controller completely
        /// </summary>
        public void Stop()
        {
            Disable();
            Proxy.Stop();
        }

    }
}
