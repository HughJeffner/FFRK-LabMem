using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace FFRK_LabMem.Services
{
    public class Proxy
    {

        public interface IProxyMachine
        {
            /// <summary>
            /// Gives a chance for this machine to register with the proxy
            /// </summary>
            /// <param name="Proxy"></param>
            void RegisterWithProxy(Proxy Proxy);

            /// <summary>
            /// Data mached from registrations is passed from the proxy to the machine
            /// </summary>
            /// <param name="UrlContains"></param>
            /// <param name="data"></param>
            Task PassFromProxy(int id, String urlMatch, JObject data);

        }

        public event EventHandler<ProxyEventArgs> ProxyEvent;

        public class Registration
        {
            public Regex UrlMatch { get; set; }
            public IProxyMachine Machine { get; set; }
        }

        public class ProxyEventArgs{
            public String Url { get; set; }
            public String Body { get; set; }
        }
               
        ProxyServer proxyServer = null;
        ExplicitProxyEndPoint explicitEndPoint = null;
        public List<Registration> Registrations {get; set;}
        private bool debug;
        private bool secure;

        public Proxy(int port, bool secure, bool debug)
        {
            this.debug = debug;
            this.secure = secure;
            this.Registrations = new List<Registration>();

            // Proxy Setup
            proxyServer = new ProxyServer(false);
            proxyServer.EnableConnectionPool = false;

            // Proxy Root Cert - Long lived
            proxyServer.CertificateManager.CertificateValidDays = 365 * 10;
            proxyServer.CertificateManager.CreateRootCertificate();

            // Generated Certificates - Short Lived
            proxyServer.CertificateManager.CertificateValidDays = 30;

            proxyServer.BeforeResponse += OnResponse;
            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, secure)
            {
                // Use self-issued generic certificate on all https requests
                // Optimizes performance by not creating a certificate for each https-enabled domain
                // Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };
            explicitEndPoint.BeforeTunnelConnectRequest += onBeforeTunnelConnectRequest;

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);
            
        }

        public void Start()
        {
            ColorConsole.WriteLine("Starting proxy server on {0}:{1} https:{2}", proxyServer.ProxyEndPoints[0].IpAddress, proxyServer.ProxyEndPoints[0].Port, secure);
            proxyServer.Start();
            
        }

        public void Stop()
        {
            //proxyServer.BeforeResponse -= OnResponse;
            //explicitEndPoint.BeforeTunnelConnectRequest -= onBeforeTunnelConnectRequest;
            proxyServer.Stop();
        }

        public void AddRegistration(String UrlMatch, IProxyMachine Machine)
        {
            this.Registrations.Add(new Registration(){ 
                UrlMatch = new Regex(UrlMatch),
                Machine = Machine
            });
        }

        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            //var responseHeaders = e.HttpClient.Response.Headers;
            System.Diagnostics.Debug.Print(e.HttpClient.Request.Url);
            if (this.debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, e.HttpClient.Request.Url);
            if (!e.HttpClient.Request.Host.Equals("ffrk.denagames.com")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("application/json"))
                    {

                        if (Registrations.Any(r => r.UrlMatch.Match(e.HttpClient.Request.Url).Success))
                        {
                            string body = await e.GetResponseBodyAsString();
                            ProxyEvent(sender, new ProxyEventArgs() { Body = body, Url = e.HttpClient.Request.Url });

                        }
                    }
                }
            }
        }


        private Task onBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            // e.GetState().PipelineInfo.AppendLine(nameof(onBeforeTunnelConnectRequest) + ":" + hostname);

            //var clientLocalIp = e.ClientLocalEndPoint.Address;
            //if (!clientLocalIp.Equals(IPAddress.Loopback) && !clientLocalIp.Equals(IPAddress.IPv6Loopback))
            //{
            //    e.HttpClient.UpStreamEndPoint = new IPEndPoint(clientLocalIp, 0);
            //}

            if (!hostname.Contains("ffrk.denagames.com"))
            {
                e.DecryptSsl = false;
                System.Diagnostics.Debug.Print("Tunnel to: " + hostname);
                if (this.debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "Tunnel to: " + hostname);
            }
            return Task.FromResult(false);
        }
           
    }
}
