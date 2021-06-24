using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace FFRK_LabMem
{
    class Program
    {
        static void Main(string[] args)
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(@"C:\Users\JLingis\SDK\android\platform-tools\adb.exe", restartServerIfNewer: false);
            Console.WriteLine(result);
            AdbClient.Instance.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 7555));
            var device = AdbClient.Instance.GetDevices().First();
            if (device != null)
            {
                Console.WriteLine("Connected to " + device.Name);
                var lab = new Lab(device);

            }
            else
            {
                Console.WriteLine("Could not connect");
            }

            var proxyServer = new ProxyServer();
            proxyServer.BeforeResponse += OnResponse;
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8081, false)
            {
                // Use self-issued generic certificate on all https requests
                // Optimizes performance by not creating a certificate for each https-enabled domain
                // Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            // Only explicit proxies can be set as system proxy!
           // proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            
            Console.ReadKey();

            proxyServer.BeforeResponse -= OnResponse;
            proxyServer.Stop();

        }

        public static async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            var responseHeaders = e.HttpClient.Response.Headers;
            Console.WriteLine(e.HttpClient.Request.Url);
            //if (!e.ProxySession.Request.Host.Equals("medeczane.sgk.gov.tr")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = await e.GetResponseBody();
                        e.SetResponseBody(bodyBytes);

                        string body = await e.GetResponseBodyAsString();
                        e.SetResponseBodyString(body);
                    }
                }
            }

        }
    }
}
