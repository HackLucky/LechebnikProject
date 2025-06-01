using System.Windows;

namespace LechebnikProject
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppConfigManager.InitializeConnectionString();
        }
    }
}
