using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private string _searchText;
        public ObservableCollection<object> CombinedList { get; set; }
        public object SelectedItem { get; set; }
        public decimal SelectedDiscount { get; set; }
        public ICommand AddUserCommand { get; }
        public ICommand ChangeRoleCommand { get; }
        public ICommand BlockUserCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ChangeDiscountCommand { get; }

        public ManageUsersViewModel()
        {
            LoadCombinedList();
            AddUserCommand = new RelayCommand(AddUser);
            ChangeRoleCommand = new RelayCommand(ChangeRole);
            BlockUserCommand = new RelayCommand(BlockUser);
            GoBackCommand = new RelayCommand(GoBack);
            ChangeDiscountCommand = new RelayCommand(ChangeDiscount);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadCombinedList(_searchText);
            }
        }

        private void LoadCombinedList(string searchText = "")
        {
            CombinedList = new ObservableCollection<object>();
            string userQuery = "SELECT * FROM Users WHERE Login LIKE @SearchText OR LastName LIKE @SearchText";
            string clientQuery = "SELECT * FROM Clients WHERE Login LIKE @SearchText OR LastName LIKE @SearchText";
            var parameters = new[] { new SqlParameter("@SearchText", $"%{searchText}%") };

            DataTable userTable = DatabaseHelper.ExecuteQuery(userQuery, parameters);
            foreach (DataRow row in userTable.Rows)
            {
                CombinedList.Add(new
                {
                    Type = "User",
                    UserId = row.Field<int>("UserId"),
                    Login = row.Field<string>("Login"),
                    LastName = row.Field<string>("LastName"),
                    Role = row.Field<string>("Role"),
                    Status = row.Field<string>("Status"),
                    Discount = "N/A"
                });
            }

            DataTable clientTable = DatabaseHelper.ExecuteQuery(clientQuery, parameters);
            foreach (DataRow row in clientTable.Rows)
            {
                CombinedList.Add(new
                {
                    Type = "Client",
                    UserId = row.Field<int>("ClientId"),
                    Login = row.Field<string>("Login"),
                    LastName = row.Field<string>("LastName"),
                    Role = "Client",
                    Status = row.Field<string>("Status"),
                    Discount = row.Field<decimal>("Discount").ToString("F2") + "%"
                });
            }
        }

        private void AddUser(object parameter) { /* Реализация добавления */ }

        private void ChangeRole(object parameter)
        {
            if (SelectedItem == null) return;
            var item = SelectedItem as dynamic;
            if (item.Type == "Client")
            {
                MessageBox.Show("Нельзя изменить роль клиента!");
                return;
            }
            // Логика смены роли для пользователей
        }

        private void BlockUser(object parameter)
        {
            if (SelectedItem == null) return;
            var item = SelectedItem as dynamic;
            string newStatus = item.Status == "Active" ? "Blocked" : "Active";
            string query = item.Type == "User" ?
                "UPDATE Users SET Status = @Status WHERE UserId = @Id" :
                "UPDATE Clients SET Status = @Status WHERE ClientId = @Id"; // Примечание: Нужно добавить колонку Status в Clients
            var parameters = new[]
            {
                new SqlParameter("@Status", newStatus),
                new SqlParameter("@Id", item.UserId)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            LoadCombinedList();
        }

        private void ChangeDiscount(object parameter)
        {
            if (SelectedItem == null) return;
            var item = SelectedItem as dynamic;
            if (item.Type == "User")
            {
                MessageBox.Show("Нельзя установить скидку для пользователя или администратора!");
                return;
            }
            string query = "UPDATE Clients SET Discount = @Discount WHERE ClientId = @Id";
            var parameters = new[]
            {
                new SqlParameter("@Discount", SelectedDiscount),
                new SqlParameter("@Id", item.UserId)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            LoadCombinedList();
        }

        private void GoBack(object parameter)
        {
            var adminPanelWindow = new AdminPanelWindow();
            adminPanelWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is ManageUsersWindow))?.Close();
        }
    }
}