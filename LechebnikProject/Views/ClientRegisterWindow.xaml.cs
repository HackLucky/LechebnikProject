using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientRegisterWindow : Window
    {
        private readonly ClientRegisterViewModel _viewModel;
        public string RegisteredLogin { get; private set; }
        public string RegisteredCode { get; private set; }

        public ClientRegisterWindow()
        {
            InitializeComponent();
            _viewModel = new ClientRegisterViewModel();
            DataContext = _viewModel;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LastName = LastNameTextBox.Text;
            _viewModel.FirstName = FirstNameTextBox.Text;
            _viewModel.MiddleName = MiddleNameTextBox.Text;
            _viewModel.PhoneNumber = PhoneNumberTextBox.Text;
            _viewModel.Email = EmailTextBox.Text;
            _viewModel.Login = LoginTextBox.Text;
            _viewModel.Code = CodePasswordBox.Password;
            _viewModel.CaptchaInput = CaptchaTextBox.Text;

            if (_viewModel.Register())
            {
                RegisteredLogin = _viewModel.Login;
                RegisteredCode = _viewModel.Code;
                DialogResult = true;
                Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var clientLoginWindow = new ClientLoginWindow();
            clientLoginWindow.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}