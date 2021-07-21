using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;


namespace FFRK_LabMem
{
    class LabController
    {

        private bool enabled = true;
        public Lab Lab { get; set; }
        public Proxy Proxy { get; set; }
        public Adb Adb { get; set; }
        private BlockingCollection<Proxy.ProxyEventArgs> queue = new BlockingCollection<Proxy.ProxyEventArgs>();

        public LabController(String adbPath, String adbHost, int proxyPort, int topOffset, int bottomOffset, String configFile)
        {

            // Proxy Server
            this.Proxy = new Proxy(proxyPort);
            this.Proxy.ProxyEvent += Proxy_ProxyEvent;
            this.Proxy.Start();
            
            // Adb
            this.Adb = new Adb(adbPath, adbHost, topOffset, bottomOffset);

            // Start if connected
            if (this.Adb.Connect().Result)
            {
                ColorConsole.WriteLine("Setting up Lab with config: {0}", configFile);
                this.Lab = new Lab(this.Adb, JsonConvert.DeserializeObject<Lab.Configuration>(File.ReadAllText(configFile)));
                this.Lab.LabFinished += Lab_LabFinished;
                this.Lab.RegisterWithProxy(this.Proxy);
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

        void Lab_LabFinished(object sender, EventArgs e)
        {
            Disable();
        }

        void Proxy_ProxyEvent(object sender, Proxy.ProxyEventArgs e)
        {
            if (enabled) queue.Add(e);
        }

        public void Enable()
        {
            if (!enabled)
            {
                enabled = true;
                ColorConsole.WriteLine(ConsoleColor.Green, "Enabled");
            }

        }

        public void Disable()
        {

            if (enabled)
            {
                enabled = false;
                ColorConsole.WriteLine(ConsoleColor.Red, "Disabled");
            }
        }

        public void Stop()
        {
            Disable();
            this.Proxy.Stop();
        }

    }
}
