using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientAuthWindow : Window
    {
        private readonly ClientLoginViewModel _viewModel;
        public Client AuthenticatedClient { get; private set; }

        public ClientAuthWindow()
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
                // Получаем данные клиента
                string query = "SELECT * FROM Clients WHERE Login = @Login";
                var parameters = new[] { new SqlParameter("@Login", _viewModel.Login) };
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                var row = dataTable.Rows[0];
                AuthenticatedClient = new Client
                {
                    ClientId = row.Field<int>("ClientId"),
                    Login = row.Field<string>("Login"),
                    Discount = row.Field<decimal>("Discount")
                };
                DialogResult = true;
                Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new ClientRegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                LoginTextBox.Text = registerWindow.RegisteredLogin;
                CodePasswordBox.Password = registerWindow.RegisteredCode;
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}