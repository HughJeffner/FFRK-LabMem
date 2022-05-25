using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FFRK_LabMem.Config.UI;

namespace FFRK_LabMem
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //desktop.MainWindow = new ConfigWindow();
            }
            base.OnFrameworkInitializationCompleted();
        }

        public static void Shutdown()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                Dispatcher.UIThread.InvokeAsync(() => lifetime.Shutdown());
            }
        }
        
    }
}
