using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NP.NLogAdapter;
using NP.Utilities;

namespace NP.Demos.MultiPlatformWindowDemo
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new StartupTestWindow();

                desktop.MainWindow.Classes.Add("PlainCustomWindow");
            }

            NLogWrapper.SetLog();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
