using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FFRK_Machines.Services;
using FFRK_Machines.Services.Adb;
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

        public class StartArguments
        {
            public string AdbPath { get; set; } = "adb.exe";
            public string AdbHost { get; set; } = "127.0.0.1:5555";
            public int TapDelay { get; set; } = 30;
            public int TapDuration { get; set; } = 0;
            public int TapPressure { get; set; } = 50;
            public int ProxyPort { get; set; } = 8081;
            public bool ProxySecure { get; set; } = true;
            public string ProxyBlocklist { get; set; }
            public bool ProxyAutoConfig { get; set; } = false;
            public bool ProxyConnectionPooling { get; set; } = false;
            public int TopOffset { get; set; } = -1;
            public int BottomOffset { get; set; } = -1;
            public Adb.CaptureType Capture { get; set; } = Services.Adb.Adb.CaptureType.Minicap;
            public int CaptureRate { get; set; } = 200;
            public double FindPrecision { get; set; } = 0.5;
            public int FindAccuracy { get; set; } = 0;
            public Adb.InputType Input { get; set; } = Adb.InputType.Minitouch;
            public string ConfigFile { get; set; }
            public int Consumers { get; set; } = 2;
            public String ScreenshotFolder { get; set; } = "";
        }

        private const char BOMChar = (char)65279;

        public event EventHandler OnEnabled;
        public event EventHandler OnDisabled;

        private bool enabled = false;
        private StartArguments startArguments;
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
        public async Task Start(StartArguments args)
        {
            // Args
            this.startArguments = args;

            // Adb
            this.Adb = new Adb(args.AdbPath, args.AdbHost, args.TopOffset, args.BottomOffset);
            this.Adb.DeviceUnavailable += Adb_DeviceUnavailable;
            this.Adb.Capture = args.Capture;
            this.Adb.CaptureRate = args.CaptureRate;
            this.Adb.FindPrecision = args.FindPrecision;
            this.Adb.FindAccuracy = args.FindAccuracy;
            this.Adb.Input = args.Input;
            this.Adb.TapDelay = args.TapDelay;
            this.Adb.TapDuration = args.TapDuration;
            this.Adb.TapPressure = args.TapPressure;
            this.Adb.ScreenshotFolder = args.ScreenshotFolder;

            // Proxy Server
            Proxy = new Proxy(args.ProxyPort, args.ProxySecure, args.ProxyBlocklist, args.ProxyConnectionPooling);
            this.Proxy.ProxyEvent += Proxy_ProxyEvent;
            Proxy.Start();

            // Machine
            ColorConsole.WriteLine("Setting up {0} with config: {1}", typeof(M).Name, args.ConfigFile);
            Machine = this.CreateMachine(await MachineConfiguration.Load<C>(args.ConfigFile));
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
            for (int i = 0; i < args.Consumers; i++)
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
                    JObject data = null;
                    if (item.ContentType.Contains("application/json"))
                    {
                        if (item.Body[0].Equals(BOMChar)) item.Body = item.Body.Substring(1); // Strip BOM if present
                        data = JObject.Parse(item.Body);
                    } else
                    {
                        // Extract json from html
                        string marker = "<script data-app-init-data type=\"application/json\">";
                        int start = item.Body.IndexOf(marker);
                        if (start > 0)
                        {
                            start += marker.Length;
                            int end = item.Body.IndexOf("</script>", start);
                            if (end > 0)
                            {
                                data = JObject.Parse(item.Body.Substring(start, end - start));
                            }
                        }
                    }
                    await item.Registration.Handler(new Proxy.RegistrationHandlerArgs()
                    {
                        Data = data,
                        Url = item.Registration.UrlMatch.ToString(),
                        Body = item.Body
                    });

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
            if (startArguments.ProxySecure) await Adb.InstallCertificate("rootCert.pfx", CancellationToken.None);
            if (startArguments.ProxyAutoConfig) await Adb.SetProxySettings(this.Proxy.Port, CancellationToken.None);
            await Adb.CaptureSetup(CancellationToken.None);
            await Adb.InputSetup(CancellationToken.None);
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
        async void Machine_MachineError(object sender, Exception e)
        {
            ColorConsole.WriteLine(ConsoleColor.Red, "Something unexpected happened ({0}).  To prevent damages, {1} will now be reset.  Please re-enable when ready with 'E'", e.Message, typeof(M).Name);
            Disable();
            await Machine.Notify(Services.Notifications.Notifications.EventType.LAB_FAULT, $"Unhandled Exception: {e.Message}");
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

        public void Refresh()
        {
            if (enabled && Machine != null)
            {
                Disable();
                Enable();
            }
        }

    }
}
