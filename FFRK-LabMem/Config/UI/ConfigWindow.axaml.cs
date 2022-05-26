using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using FFRK_Machines.Services.Notifications;

namespace FFRK_LabMem.Config.UI
{
    public partial class ConfigWindow : Window
    {
        public static bool IsLoaded { get; set; } = false;

        private AppConfig config = null;
        private LabController controller = null;
        private LabConfiguration labConfig = new LabConfiguration();
        private int initalTabIndex = 0;
        protected Scheduler scheduler = null;
        private Notifications.EventList notificationEvents = null;
        private Notifications.EventType? selectedNotificationEvent = null;

        public ConfigWindow()
        {
            InitializeComponent();
            Closed += ConfigWindow_Closed;
            var button = this.FindControl<Button>("ButtonCancel");
            button.Click += (sender,e) => this.Close();
        }

        private void ConfigWindow_Closed(object sender, System.EventArgs e)
        {
            IsLoaded = false;
        }

        public static void CreateAndShow(AppConfig config, LabController controller, int initalTabIndex = 0)
        {
            if (IsLoaded == false)
            {
                IsLoaded = true;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var form = new ConfigWindow()
                    {
                        config = config,
                        controller = controller,
                        scheduler = Scheduler.Default,
                        initalTabIndex = initalTabIndex
                    };
                    form.Show();
                });

            }
        }
    }
}
