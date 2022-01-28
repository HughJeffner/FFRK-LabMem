using FFRK_Machines;
using FFRK_Machines.Services;
using System;
using System.Threading;

namespace FFRK_LabMem.Services
{

    class Clipboard
    {

        public static void CopyProxyBypassToClipboard()
        {

            Thread thread = new Thread(() => {
                System.Windows.Forms.Clipboard.SetText(Proxy.PROXY_BYPASS);
                ColorConsole.WriteLine(ConsoleColor.Gray, "Copied proxy bypass to clipboard");
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

    }
}
