﻿using FFRK_LabMem.Config.UI;
using FFRK_LabMem.Data.UI;
using FFRK_LabMem.Machines;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        enum MonitorState
        {
            ON = -1,
            OFF = 2,
            STANDBY = 1
        }

        static NotifyIcon notifyIcon = null;

        public static void MinimizeTo(ConsoleModifiers modifiers, LabController controller)
        {
            Tray.MinimizeTo(controller, modifiers.HasFlag(ConsoleModifiers.Alt), modifiers.HasFlag(ConsoleModifiers.Control));
        }

        public static void MinimizeTo(LabController controller, bool monitorOff = false, bool lockWorkstation = false){

            // Windows API to hide window
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            // Init tray icon from windows forms
            if (notifyIcon == null)
            {
                Task mytask = Task.Run(() =>
                {
                    // Create and set properties
                    notifyIcon = new NotifyIcon();
                    notifyIcon.DoubleClick += notifyIcon_DoubleClick;
                    notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    notifyIcon.Text = Console.Title;
                    var contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add("Unhide", null, (s, e) => { notifyIcon_DoubleClick(s, e); });
                    contextMenu.Items.Add("Stats", null, (s, e) => {
                        CountersForm.CreateAndShow(controller);
                    });
                    contextMenu.Items.Add("Config", null, (s, e) => {
                        ConfigForm.CreateAndShow(new Config.ConfigHelper(), controller);
                    });
                    contextMenu.Items.Add("-");
                    contextMenu.Items.Add("Exit", null, (s, e) => {
                        if (MessageBox.Show("Are you sure you wish to exit?", "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK) Environment.Exit(0);
                    });
                    notifyIcon.ContextMenuStrip = contextMenu;
                    notifyIcon.Visible = true;

                    // From this point forward a message loop will run on this thread that owns the notifyIcon
                    System.Threading.Thread.CurrentThread.Name = "Message Pump";
                    Application.Run();
                });
            }
            else
            {
                notifyIcon.Visible = true;
            }

            // Super hide
            if (monitorOff)
            {
                // Delay to keep key release from waking monitor
                Task.Delay(1000).ContinueWith(t => SendMessage(GetConsoleWindow(), WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MonitorState.OFF));
                
            }

            // Lock
            if (lockWorkstation) Task.Delay(1000).ContinueWith( t => LockWorkStation());

        }

        private static void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            
            // Windows API to show window
            ShowWindow(GetConsoleWindow(), SW_SHOW);
            notifyIcon.Visible = false;

        }

    }
}
