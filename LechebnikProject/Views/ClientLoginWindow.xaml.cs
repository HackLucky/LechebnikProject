using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientLoginWindow : Window
    {
        public readonly ClientLoginViewModel _viewModel;

        public ClientLoginWindow()
        {
            InitializeComponent();
            LoginTextBox.Focus();
            _viewModel = new ClientLoginViewModel();
            DataContext = _viewModel;
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Authenticate())
            {
                DialogResult = true;
                Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var clientRegisterWindow = new ClientRegisterWindow();
            if (clientRegisterWindow.ShowDialog() == true)
            {
                _viewModel.Login = clientRegisterWindow.RegisteredLogin;
                _viewModel.Code = clientRegisterWindow.RegisteredCode;
                if (_viewModel.Authenticate())
                {
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}