using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
            
            // Windows API to hide window
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            // Init tray icon from windows forms
            if (notifyIcon == null)
            {

                notifyIcon = new NotifyIcon();
                notifyIcon.DoubleClick +=notifyIcon_DoubleClick;
                notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                notifyIcon.Text = Application.ProductName;

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Show", null, (s, e) => { notifyIcon_DoubleClick(s, e); });
                notifyIcon.ContextMenuStrip = contextMenu;

                notifyIcon.Visible = true;

            }
            else
            {
                notifyIcon.Visible = true;
            }

            // Message pump to handle icon events - console will pause
            Application.Run(); 

        }

        private static void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            
            // Windows API to show window
            ShowWindow(GetConsoleWindow(), SW_SHOW);
            notifyIcon.Visible = false;

            // Stop message pump, resume console
            Application.Exit();
        }

    }
}
