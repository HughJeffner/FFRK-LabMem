using FFRK_Machines;
using System;
using System.Threading;

namespace FFRK_LabMem.Services
{

    class Clipboard
    {
        private const string PROXY_BYPASS = "127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io";

        public static void CopyProxyBypassToClipboard()
        {

            Thread thread = new Thread(() => {
                System.Windows.Forms.Clipboard.SetText(PROXY_BYPASS);
                ColorConsole.WriteLine(ConsoleColor.Gray, "Copied proxy bypass to clipboard");
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

    }
}
