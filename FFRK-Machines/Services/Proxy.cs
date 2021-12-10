using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

        public const string PROXY_BYPASS = "127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io";

        public interface IProxyMachine
        {
            /// <summary>
            /// Gives a chance for this machine to register with the proxy
            /// </summary>
            /// <param name="Proxy"></param>
            void RegisterWithProxy(Proxy Proxy);

        }

        public event EventHandler<ProxyEventArgs> ProxyEvent;

        public class Registration
        {
            public Regex UrlMatch { get; set; }
            public Func<JObject,string,Task> Handler { get; set; }
        }

        public class ProxyEventArgs{
            public string Url { get; set; }
            public string Body { get; set; }
            public Registration Registration { get; set; }
            public string ContentType { get; set; }
        }

        readonly ProxyServer proxyServer = null;
        readonly ExplicitProxyEndPoint explicitEndPoint = null;
        public List<Registration> Registrations { get; set; } = new List<Registration>();
        private bool secure;
        public int Port { get; set; }
        private List<string> Blocklist { get; set; } = new List<string>();

        public Proxy (int port, bool secure) : this(port, secure, null, false) { }

        public Proxy(int port, bool secure, string blocklist, bool connectionPooling)
        {
            this.secure = secure;
            this.Port = port;

            // Proxy Setup
            proxyServer = new ProxyServer(false);
            proxyServer.EnableConnectionPool = connectionPooling;

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
            explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);

            // Blocklist
            if (!string.IsNullOrEmpty(blocklist) && File.Exists(blocklist))
            {
                Blocklist = new List<string>(File.ReadAllLines(blocklist));
                proxyServer.BeforeRequest += BeforeRequest;
            }

            // Upstream proxy for debug purposes
            //proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };

        }

        public void Start()
        {
            var ip = Proxy.GetMainIPv4();
            var ipString = "0.0.0.0";
            if (ip != null)
            {
                ipString = ip.ToString();
            }
            ColorConsole.WriteLine("Starting proxy server on {0}:{1} https:{2}", ipString, proxyServer.ProxyEndPoints[0].Port, secure);
            proxyServer.Start();
            
        }

        public void Stop()
        {
            //proxyServer.BeforeResponse -= OnResponse;
            //explicitEndPoint.BeforeTunnelConnectRequest -= onBeforeTunnelConnectRequest;
            proxyServer.Stop();
        }

        public void AddRegistration(string UrlMatch, Func<JObject,string,Task> handler)
        {
            this.Registrations.Add(new Registration(){ 
                UrlMatch = new Regex(UrlMatch),
                Handler = handler
            });
        }

        private static bool IsIPv4(IPAddress ipa) => ipa.AddressFamily == AddressFamily.InterNetwork;

        public static IPAddress GetMainIPv4()
        {

            try
            {
                return NetworkInterface.GetAllNetworkInterfaces()
                .Select((ni) => ni.GetIPProperties())
                .Where((ip) => ip.GatewayAddresses.Where((ga) => IsIPv4(ga.Address)).Count() > 0)
                .FirstOrDefault()?.UnicastAddresses?
                .Where((ua) => IsIPv4(ua.Address))?.FirstOrDefault()?.Address;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            //var responseHeaders = e.HttpClient.Response.Headers;
            System.Diagnostics.Debug.Print(e.HttpClient.Request.Url);
            ColorConsole.Debug(ColorConsole.DebugCategory.Proxy, e.HttpClient.Request.Url);
            if (!e.HttpClient.Request.Host.Equals("ffrk.denagames.com")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    var matches = Registrations.Where(r => r.UrlMatch.Match(e.HttpClient.Request.Url).Success);
                    if (matches.Count() > 0)
                    {
                        string body = await e.GetResponseBodyAsString();
                        foreach (var match in matches)
                        {
                            ProxyEvent(sender, new ProxyEventArgs() { 
                                Body = body, 
                                Url = e.HttpClient.Request.Url, 
                                Registration = match, 
                                ContentType = e.HttpClient.Response.ContentType.Trim().ToLower() 
                            });
                        }
                    }
                }
            }
        }

        private Task BeforeRequest(object sender, SessionEventArgs e)
        {

            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (Blocklist.Any(b => hostname.EndsWith(b)))
            {
                e.TerminateSession();
                System.Diagnostics.Debug.Print("Blocked: " + hostname);
                ColorConsole.Debug(ColorConsole.DebugCategory.Proxy, "Blocked: {0}", hostname);
            }

            return Task.FromResult(true);
        }

        private Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            // e.GetState().PipelineInfo.AppendLine(nameof(onBeforeTunnelConnectRequest) + ":" + hostname);

            //var clientLocalIp = e.ClientLocalEndPoint.Address;
            //if (!clientLocalIp.Equals(IPAddress.Loopback) && !clientLocalIp.Equals(IPAddress.IPv6Loopback))
            //{
            //    e.HttpClient.UpStreamEndPoint = new IPEndPoint(clientLocalIp, 0);
            //}

            if (Blocklist.Any(b => hostname.EndsWith(b)))
            {
                e.DenyConnect = true;
                System.Diagnostics.Debug.Print("Blocked: " + hostname);
                ColorConsole.Debug(ColorConsole.DebugCategory.Proxy, "Blocked: {0}", hostname);
                return Task.FromResult(false);
            }

            if (!hostname.Contains("ffrk.denagames.com"))
            {
                e.DecryptSsl = false;
                System.Diagnostics.Debug.Print("Tunnel to: " + hostname);
                ColorConsole.Debug(ColorConsole.DebugCategory.Proxy, "Tunnel to: {0}", hostname);
            }

            return Task.FromResult(true);
        }
           
    }
}
