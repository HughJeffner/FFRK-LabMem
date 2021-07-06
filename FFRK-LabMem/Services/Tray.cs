using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRK_LabMem.Services
{
    class Tray
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static NotifyIcon notifyIcon = null;

        public static void MinimizeTo(){
            
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            if (notifyIcon == null)
            {

                notifyIcon = new NotifyIcon();
                notifyIcon.DoubleClick += (s, e) =>
                {
                    ShowWindow(GetConsoleWindow(), SW_SHOW);
                    notifyIcon.Visible = false;
                    Application.Exit();
                };
                
                notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                notifyIcon.Text = Application.ProductName;

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });
                notifyIcon.ContextMenuStrip = contextMenu;

                notifyIcon.Visible = true;

            }
            else
            {
                notifyIcon.Visible = true;
            }

            Application.Run(); 

            


        }

    }
}
