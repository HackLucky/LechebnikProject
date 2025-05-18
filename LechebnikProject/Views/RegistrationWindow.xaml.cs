using System.Windows;
using LechebnikProject.ViewModels;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private readonly RegistrationViewModel _viewModel;

        public RegistrationWindow()
        {
            InitializeComponent();
            _viewModel = new RegistrationViewModel();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Заполнение свойств ViewModel из полей ввода
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

            if (_viewModel.Register())
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
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