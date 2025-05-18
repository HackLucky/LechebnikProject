using System.Threading.Tasks;
using System.Windows;

namespace LechebnikProject.ViewModels
{
    /// <summary>
    /// ViewModel для приветственного окна, отображается 2 секунды.
    /// </summary>
    public class WelcomeViewModel
    {
        public WelcomeViewModel()
        {
            // Запуск асинхронного метода для перехода к окну авторизации через 2 секунды
            Task.Delay(2000).ContinueWith(t => OpenLoginWindow());
        }

        /// <summary>
        /// Открывает окно авторизации и закрывает текущее.
        /// </summary>
        private void OpenLoginWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var loginWindow = new Views.LoginWindow(); // Создание окна авторизации
                loginWindow.Show();                        // Отображение окна
                Application.Current.MainWindow.Close();    // Закрытие текущего окна
                Application.Current.MainWindow = loginWindow; // Установка нового главного окна
            });
        }
    }
}