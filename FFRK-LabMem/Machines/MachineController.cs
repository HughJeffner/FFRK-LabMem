using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFRK_LabMem.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FFRK_LabMem.Machines
{
    /// <summary>
    /// An abstract class that controls a machine
    /// </summary>
    /// <typeparam name="M">The machine class</typeparam>
    /// <typeparam name="S">An enum of all the possible machine states</typeparam>
    /// <typeparam name="T">An enum of all the available machine triggers</typeparam>
    /// <typeparam name="C">A class containing configuration information for the machine, must inherit from MachineConfiguration</typeparam>
    abstract class MachineController<M, S, T, C> 
        where M : Machine<S,T,C>
        where C : MachineConfiguration
    {

        private S unknownState;
        private bool enabled = true;
        public M Machine { get; set; }
        public Proxy Proxy { get; set; }
        public Adb Adb { get; set; }
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
        /// <param name="topOffset">Top offset of screen</param>
        /// <param name="bottomOffset">Bottom offest of screen</param>
        /// <param name="configFile">Path to the machine config file</param>
        /// <param name="unkownState">State the machine should enter if reset, or unknown state</param>
        public async Task Start(bool debug, String adbPath, String adbHost, int proxyPort, int topOffset, int bottomOffset, String configFile, S unkownState)
        {

            this.unknownState = unkownState;

            // Proxy Server
            this.Proxy = new Proxy(proxyPort, debug);
            this.Proxy.ProxyEvent += Proxy_ProxyEvent;
            this.Proxy.Start();
            
            // Adb
            this.Adb = new Adb(adbPath, adbHost, topOffset, bottomOffset);

            // Start if connected
            if (await this.Adb.Connect())
            {
                ColorConsole.WriteLine("Setting up {0} with config: {1}", typeof(M).Name, configFile);
                this.Machine = this.CreateMachine(JsonConvert.DeserializeObject<C>(File.ReadAllText(configFile)));
                this.Machine.MachineFinished += Machine_MachineFinished;
                this.Machine.MachineError += Machine_MachineError;
                this.Machine.RegisterWithProxy(this.Proxy);
            }

            // Consumer queue
            var cts = new CancellationTokenSource();
            var consumerTask = Task.Run(async() =>
            {
                foreach (var item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        var data = JObject.Parse(item.Body.Substring(1));
                        int i = 0;
                        foreach (var r in this.Proxy.Registrations)
                        {
                            var match = r.UrlMatch.Match(item.Url);
                            if (match.Success)
                                await r.Machine.PassFromProxy(i, match.Value, data);
                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                    }

                }
            });

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
            if (!enabled && this.Machine != null)
            {
                enabled = true;
                this.Machine.ConfigureStateMachine(unknownState);
                ColorConsole.WriteLine(ConsoleColor.Green, "Enabled {0}", typeof(M).Name);
            }

        }

        /// <summary>
        /// Disables the machine
        /// </summary>
        public void Disable()
        {

            if (enabled && this.Machine != null)
            {
                enabled = false;
                this.Machine.Disable();
                ColorConsole.WriteLine(ConsoleColor.Red, "Disabled {0}", typeof(M).Name);
            }

        }

        /// <summary>
        /// Stops the machine controller completely
        /// </summary>
        public void Stop()
        {
            Disable();
            this.Proxy.Stop();
        }
        
    }
}
