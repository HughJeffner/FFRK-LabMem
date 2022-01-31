using SharpAdbClient;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Adb
{
    class CertificateManager
    {
        private readonly IAdbClient client;
        private readonly DeviceData device;
        private readonly Adb adb;
        private const String CERTIFICATE_USER_PATH = "/data/misc/user/0/cacerts-added/3dcac768.0";
        private const String CERTIFICATE_SYSTEM_PATH = "/system/etc/security/cacerts/3dcac768.0";
        private const String CERTIFICATE_CRT_PATH = "/sdcard/LabMem_Root_Cert.crt";

        public class RootCertInstalledStatus
        {
            public bool Installed { get; set; }
            public bool UserExists { get; set; }
            public bool SystemExists { get; set; }
        }
        public CertificateManager(Adb adb)
        {
            this.client = AdbClient.Instance;
            this.device = adb.Device;
            this.adb = adb;
        }
        public async Task InstallRootCert(String pfxPath, CancellationToken cancellationToken)
        {

            // Get API level
            int apiLevel = await adb.GetAPILevel(cancellationToken);

            // Lollipop or higher
            if (apiLevel >= 21)
            {

                // If pfx file exists
                if (File.Exists(pfxPath))
                {

                    // Root Cert in filesystem
                    var rootCert = new X509Certificate2(pfxPath, "")
                    {
                        PrivateKey = null
                    };

                    // If needs install
                    var installStatus = await CheckIfRootCertInstalled(apiLevel);
                    if (!installStatus.Installed)
                    {
                        // Prompt to install
                        await PromptToInstallRootCert(CERTIFICATE_CRT_PATH, rootCert, apiLevel, cancellationToken);

                    }
                    else
                    {
                        // Installed certificate validation checks
                        await ValidateInstalledRootCert(CERTIFICATE_CRT_PATH, installStatus.SystemExists, rootCert, apiLevel, cancellationToken);

                    }

                }

            }

        }
        public async Task<bool> CopyUserCertsToSystem(CancellationToken cancellationToken)
        {

            var receiver = new ConsoleOutputReceiver();
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Remount system partition as writeable");
            await client.ExecuteRemoteCommandAsync("mount -o rw,remount /system",
                device,
                receiver,
                cancellationToken,
                2000);
            if (receiver.ToString().Contains("denied")) return false;
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy user certs to system certs");
            await client.ExecuteRemoteCommandAsync("cp /data/misc/user/0/cacerts-added/* /system/etc/security/cacerts/",
                device,
                receiver,
                cancellationToken,
                2000);
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Remount system partition as read only");
            await client.ExecuteRemoteCommandAsync("mount -o ro,remount /system",
               device,
               receiver,
               cancellationToken,
               2000);
            return true;
        }
        public async Task<RootCertInstalledStatus> CheckIfRootCertInstalled(int apiLevel)
        {
            using (var service = Factories.SyncServiceFactory(device))
            {
                var ret = new RootCertInstalledStatus
                {
                    UserExists = service.Stat(CERTIFICATE_USER_PATH).FileMode != 0,
                    SystemExists = service.Stat(CERTIFICATE_SYSTEM_PATH).FileMode != 0
                };
                ret.Installed = ((ret.UserExists && apiLevel <= 23) || ret.SystemExists);
                return await Task.FromResult(ret);
            }

        }
        public async Task<bool> CopyRootCertToStorage(String certPath, X509Certificate2 rootCert, CancellationToken cancellationToken)
        {

            try
            {
                using (var service = Factories.SyncServiceFactory(device))
                {
                    using (MemoryStream stream = new MemoryStream(rootCert.Export(X509ContentType.Cert)))
                    {
                        service.Push(stream, certPath, 999, DateTime.Now, null, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);

        }
        public async Task<X509Certificate2> GetInstalledRootCert(bool isSystemCert, CancellationToken cancellationToken)
        {

            var pathToCert = (isSystemCert) ? CERTIFICATE_SYSTEM_PATH : CERTIFICATE_USER_PATH;
            X509Certificate2 ret;

            using (var service = Factories.SyncServiceFactory(device))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    service.Pull(pathToCert, stream, null, cancellationToken);
                    ret = new X509Certificate2(stream.ToArray());
                }
            }

            return await Task.FromResult(ret);
        }
        public async Task PromptToInstallRootCert(String cert, X509Certificate2 rootCert, int apiLevel, CancellationToken cancellationToken)
        {

            // First copy it over
            if (await CopyRootCertToStorage(cert, rootCert, cancellationToken))
            {

                // Start security settings activity
                await client.ExecuteRemoteCommandAsync(String.Format("am start -a android.settings.SECURITY_SETTINGS"),
                                device,
                                null,
                                cancellationToken,
                                2000);

                ColorConsole.WriteLine(ConsoleColor.Yellow, "***************** Install Certificate *****************");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Scroll to Credential Storage > Install from SD card");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Browse to {0}", cert);
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Use FFRK for certificate name");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Choose VPN and Apps for credential use");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "(You may need to set a device lockscreen)");

                // Need root
                if (apiLevel >= 24)
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "*******************ROOT REQUIRED***********************");
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Please press <Enter> once certificate installed to copy");
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "it to the system store");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    if (await CopyUserCertsToSystem(cancellationToken))
                    {
                        if ((await CheckIfRootCertInstalled(apiLevel)).Installed)
                        {
                            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy complete.  You may now delete the user certificate");
                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy failed.  Check if the certificate is under the user tab of Trusted Credentials and try again");
                        }

                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "This version of android currently not supported (7+ no root).  Please root your device.");
                    }
                }
                ColorConsole.WriteLine(ConsoleColor.Yellow, "*******************************************************");

            }

        }
        public async Task ValidateInstalledRootCert(String certPath, bool isSystemCert, X509Certificate2 rootCert, int apiLevel, CancellationToken cancellationToken)
        {

            var installedRootCert = await GetInstalledRootCert(isSystemCert, cancellationToken);

            // Thumbprint check
            if (!installedRootCert.Thumbprint.Equals(rootCert.Thumbprint))
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "** CERTIFICATE MISMATCH **");
                ColorConsole.WriteLine(ConsoleColor.Red, "** DELETE THE CURRENT CERTIFICATE IN THE USER TAB BEFORE PROCEEDING**");
                await PromptToInstallRootCert(certPath, rootCert, apiLevel, cancellationToken);
                return;
            }

            // Expired
            TimeSpan timeLeft = installedRootCert.NotAfter - DateTime.Now;
            if (timeLeft.TotalMinutes <= 0)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "** CERTIFICATE EXPIRED **");
                ColorConsole.WriteLine(ConsoleColor.Red, "** DELETE THE CURRENT CERTIFICATE IN THE USER TAB BEFORE PROCEEDING**");
                await PromptToInstallRootCert(certPath, rootCert, apiLevel, cancellationToken);
                return;
            }

            // Expire warning
            if (timeLeft.Days <= 30)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "** WARNING ** Installed root CA certificate will expire in {0} days", timeLeft.Days);
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Delete .pfx file and relaunch to begin re-install process");
            }

        }

    }
}
