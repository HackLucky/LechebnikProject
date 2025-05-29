using System.Windows;

namespace LechebnikProject
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Загружаем пользовательские настройки подключения
            AppConfigManager.InitializeConnectionString();
        }
    }
}
