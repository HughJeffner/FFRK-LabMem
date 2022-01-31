using SharpAdbClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Adb
{
    class ProxyManager
    {

        public static async Task<bool> SetProxySettings(IAdbClient client, DeviceData device, int proxyPort, CancellationToken cancellationToken)
        {
            var receiver = new ConsoleOutputReceiver();

            // Remove or set
            if (proxyPort == 0)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Removing proxy settings...");
                await client.ExecuteRemoteCommandAsync("settings put global http_proxy :0",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                await client.ExecuteRemoteCommandAsync("settings delete global global_http_proxy_exclusion_list",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Please restart your device/emulator to finish applying proxy settings");
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detecting proxy settings...");
                await client.ExecuteRemoteCommandAsync("ip route list match 0 table all scope global",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                string defaultRoute = "10.0.2.2";
                if (receiver.ToString().ToLower().StartsWith("default via"))
                {
                    defaultRoute = receiver.ToString().Split(' ')[2];
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Could not determine default route, using default: {0}", defaultRoute);
                }
                receiver = new ConsoleOutputReceiver();
                await client.ExecuteRemoteCommandAsync("settings get global global_http_proxy_host",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyHostMatch = receiver.ToString().TrimEnd().ToLower().Equals(defaultRoute);
                receiver = new ConsoleOutputReceiver();
                await client.ExecuteRemoteCommandAsync("settings get global global_http_proxy_port",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyPortMatch = receiver.ToString().TrimEnd().Equals(proxyPort.ToString());
                receiver = new ConsoleOutputReceiver();
                await client.ExecuteRemoteCommandAsync("settings get global global_http_proxy_exclusion_list",
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyExclusionMatch = receiver.ToString().TrimEnd().ToLower().Equals(Proxy.PROXY_BYPASS.ToLower());
                if (!proxyHostMatch || !proxyPortMatch || !proxyExclusionMatch)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Auto-setting system proxy settings...");
                    await client.ExecuteRemoteCommandAsync(String.Format("settings put global http_proxy {0}:{1}", defaultRoute, proxyPort),
                    device,
                    receiver,
                    cancellationToken,
                    2000);
                    await client.ExecuteRemoteCommandAsync("settings put global global_http_proxy_exclusion_list " + Proxy.PROXY_BYPASS,
                        device,
                        receiver,
                        cancellationToken,
                        2000);
                    await client.ExecuteRemoteCommandAsync("settings delete global http_proxy",
                        device,
                        receiver,
                        cancellationToken,
                        2000);
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Please restart your device/emulator to apply proxy settings");
                    return await Task.FromResult(false);
                }
                else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Proxy settings OK (trouble loading?, try changing port)");
                }

            }

            return await Task.FromResult(true);

        }
    }
}
