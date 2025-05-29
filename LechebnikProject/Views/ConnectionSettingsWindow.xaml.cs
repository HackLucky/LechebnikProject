using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LechebnikProject.Views
{
    public partial class ConnectionSettingsWindow : Window
    {
        public ConnectionSettingsWindow()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            string connectionString = AppConfigManager.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString)) return;

            var builder = new SqlConnectionStringBuilder(connectionString);

            ServerTextBox.Text = builder.DataSource;
            DatabaseTextBox.Text = builder.InitialCatalog;

            if (builder.IntegratedSecurity)
            {
                WindowsAuthRadio.IsChecked = true;
                SqlAuthPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SqlAuthRadio.IsChecked = true;
                SqlAuthPanel.Visibility = Visibility.Visible;
                UsernameTextBox.Text = builder.UserID;
            }
        }

        private void AuthRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SqlAuthPanel.Visibility = SqlAuthRadio.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool useWindowsAuth = WindowsAuthRadio.IsChecked == true;
            string username = useWindowsAuth ? null : UsernameTextBox.Text;
            string password = useWindowsAuth ? null : PasswordBox.Password;

            try
            {
                AppConfigManager.UpdateConnectionString(
                    ServerTextBox.Text,
                    DatabaseTextBox.Text,
                    useWindowsAuth,
                    username,
                    password);

                MessageBox.Show("Настройки подключения успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                MessageBox.Show("Настройки подключения изменены. Перезапустите программу для применения изменений.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
