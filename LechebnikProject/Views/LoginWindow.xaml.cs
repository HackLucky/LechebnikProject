using System.Windows;
using LechebnikProject.ViewModels;
using LechebnikProject.Models;

namespace LechebnikProject.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Login = LoginTextBox.Text;
            _viewModel.Password = PasswordBox.Password;

            User user = _viewModel.Authenticate();
            if (user != null)
            {
                AppContext.CurrentUser = user;
                var mainMenuWindow = new MainMenuWindow();
                mainMenuWindow.Show();
                this.Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var connectionWindow = new ConnectionSettingsWindow();
            connectionWindow.Show();
            this.Close();
        }
    }
}