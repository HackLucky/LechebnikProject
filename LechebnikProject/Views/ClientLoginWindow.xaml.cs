using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientLoginWindow : Window
    {
        private readonly ClientLoginViewModel _viewModel;

        public ClientLoginWindow()
        {
            InitializeComponent();
            _viewModel = new ClientLoginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Login = LoginTextBox.Text;
            _viewModel.Code = CodePasswordBox.Password;
            if (_viewModel.Authenticate())
            {
                MessageBox.Show("Авторизация клиента успешна!");
                // Здесь можно открыть окно клиента, например, ClientMainWindow
                this.Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new ClientRegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}