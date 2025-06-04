using LechebnikProject.ViewModels;
using System;
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
            try
            {
                if (_viewModel.Authenticate())
                {
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
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}