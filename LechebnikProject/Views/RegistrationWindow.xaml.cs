using LechebnikProject.ViewModels;
using System;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class RegistrationWindow : Window
    {
        private readonly RegistrationViewModel _viewModel;

        public RegistrationWindow()
        {
            InitializeComponent();
            LastNameTextBox.Focus();
            _viewModel = new RegistrationViewModel();
            DataContext = _viewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.LastName = LastNameTextBox.Text;
                _viewModel.FirstName = FirstNameTextBox.Text;
                _viewModel.MiddleName = MiddleNameTextBox.Text;
                _viewModel.PhoneNumber = PhoneNumberTextBox.Text;
                _viewModel.Email = EmailTextBox.Text;
                _viewModel.Position = PositionTextBox.Text;
                _viewModel.PharmacyAddress = PharmacyAddressTextBox.Text;
                _viewModel.Login = LoginTextBox.Text;
                _viewModel.Password = PasswordBox.Password;
                _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
                _viewModel.CaptchaInput = CaptchaTextBox.Text;

                if (_viewModel.Register())
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}