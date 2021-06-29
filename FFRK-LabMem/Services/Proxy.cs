using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace FFRK_LabMem.Services
{
    class Proxy
    {

        public delegate void PaintingsLoaded(JArray paintings);
        public event PaintingsLoaded OnPaintingsLoaded;

        ProxyServer proxyServer = null;
        ExplicitProxyEndPoint explicitEndPoint = null;
        public Proxy()
        {
            proxyServer = new ProxyServer();
            proxyServer.BeforeResponse += OnResponse;
            
            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8081, false)
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
            Console.WriteLine("Starting proxy server on {0}:{1}", proxyServer.ProxyEndPoints[0].IpAddress, proxyServer.ProxyEndPoints[0].Port);
            proxyServer.Start();
            
        }

        public void Stop()
        {
            proxyServer.BeforeResponse -= OnResponse;
            explicitEndPoint.BeforeTunnelConnectRequest -= onBeforeTunnelConnectRequest;
            proxyServer.Stop();
        }

        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            var responseHeaders = e.HttpClient.Response.Headers;
            Console.WriteLine(e.HttpClient.Request.Url);
            if (!e.HttpClient.Request.Host.Equals("ffrk.denagames.com")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("application/json"))
                    {
                        string body = await e.GetResponseBodyAsString();
                        System.Diagnostics.Debug.Print(body);

                        if (e.HttpClient.Request.Url.Contains("get_display_paintings"))
                        {
                            try
                            {
                                await Task.Factory.StartNew(() =>
                                {
                                    var data = JObject.Parse(body.Substring(1));
                                    OnPaintingsLoaded((JArray)data["labyrinth_dungeon_session"]["display_paintings"]);
                                }).ConfigureAwait(false);
                                                              
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            
                        }

                    }
                }
            }

        }

        private async Task onBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
           // e.GetState().PipelineInfo.AppendLine(nameof(onBeforeTunnelConnectRequest) + ":" + hostname);
            //writeToConsole("Tunnel to: " + hostname);
            Console.WriteLine("Tunnel to: " + hostname);

            var clientLocalIp = e.ClientLocalEndPoint.Address;
            if (!clientLocalIp.Equals(IPAddress.Loopback) && !clientLocalIp.Equals(IPAddress.IPv6Loopback))
            {
                e.HttpClient.UpStreamEndPoint = new IPEndPoint(clientLocalIp, 0);
            }

            //if (hostname.Contains("dropbox.com"))
            //{
                // Exclude Https addresses you don't want to proxy
                // Useful for clients that use certificate pinning
                // for example dropbox.com
                e.DecryptSsl = false;
            //}
        }
           
    }
}
