using LechebnikProject.ViewModels;
using System;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientLoginWindow : Window
    {
        public readonly ClientLoginViewModel _viewModel;
        public string AuthLogin { get; private set; }
        public string AuthCode { get; private set; }

        public ClientLoginWindow()
        {
            InitializeComponent();
            LoginTextBox.Focus();
            _viewModel = new ClientLoginViewModel();
            DataContext = _viewModel;
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.Login = LoginTextBox.Text;
                _viewModel.Code = CodePasswordBox.Password;
                if (_viewModel.Authenticate())
                {
                    AuthLogin = _viewModel.Login;
                    AuthCode = _viewModel.Code;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clientRegisterWindow = new ClientRegisterWindow();
                bool? result = clientRegisterWindow.ShowDialog();
                if (result == true)
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
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}