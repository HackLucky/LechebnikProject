using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
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
        public ObservableCollection<BaseUser> AllEntities { get; set; } = new ObservableCollection<BaseUser>();

        private BaseUser _selectedEntity;
        public BaseUser SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                SetProperty(ref _selectedEntity, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand DelUserCommand { get; }
        public ICommand ChangeRoleCommand { get; }
        public ICommand ChangeDiscountCommand { get; }
        public ICommand ToggleBlockCommand { get; }
        public ICommand GoBackCommand { get; }

        public ManageUsersViewModel()
        {
            DelUserCommand = new RelayCommand(DeleteEntity);
            ToggleBlockCommand = new RelayCommand(ToggleBlock, o => SelectedEntity != null);
            ChangeRoleCommand = new RelayCommand(ChangeRole, o => SelectedEntity is User);
            ChangeDiscountCommand = new RelayCommand(ChangeDiscount, o => SelectedEntity is Client);
            GoBackCommand = new RelayCommand(_ => WindowManager.ShowWindow<AdminPanelWindow>());
            LoadAllData();
        }

        private void LoadAllData()
        {
            AllEntities.Clear();
            LoadUsers();
            LoadClients();
            OnPropertyChanged(nameof(AllEntities));
        }

        private void LoadUsers()
        {
            try
            {
                string query = "SELECT UserId, LastName, FirstName, MiddleName, Login, Role, Status FROM Users";
                DataTable table = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in table.Rows)
                {
                    var u = new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        LastName = row["LastName"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"] != DBNull.Value ? row["MiddleName"].ToString() : null,
                        Login = row["Login"].ToString(),
                        Role = row["Role"].ToString(),
                        Status = row["Status"].ToString()
                    };
                    AllEntities.Add(u);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void LoadClients()
        {
            try
            {
                string query = "SELECT ClientId, LastName, FirstName, MiddleName, Login, Discount, Status FROM Clients";
                DataTable table = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in table.Rows)
                {
                    var c = new Client
                    {
                        ClientId = Convert.ToInt32(row["ClientId"]),
                        LastName = row["LastName"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"] != DBNull.Value ? row["MiddleName"].ToString() : null,
                        Login = row["Login"].ToString(),
                        Discount = Convert.ToDecimal(row["Discount"]),
                        Status = row["Status"].ToString()
                    };
                    AllEntities.Add(c);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void DeleteEntity(object parameter)
        {
            try
            {
                if (SelectedEntity == null) return;

                if (SelectedEntity is User pu)
                {
                    string query = "DELETE FROM Users WHERE UserId = @Id";
                    var prm = new[] { new SqlParameter("@Id", pu.UserId) };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    MessageBox.Show($"Пользователь '{pu.Login}' удалён.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (SelectedEntity is Client cu)
                {
                    string query = "DELETE FROM Clients WHERE ClientId = @Id";
                    var prm = new[] { new SqlParameter("@Id", cu.ClientId) };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    MessageBox.Show($"Клиент '{cu.Login}' удалён.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ToggleBlock(object parameter)
        {
            try
            {
                if (SelectedEntity == null) return;

                if (SelectedEntity is User pu)
                {
                    var newStatus = pu.Status == "Active" ? "Blocked" : "Active";
                    string query = "UPDATE Users SET Status = @Status WHERE UserId = @Id";
                    var prm = new[]
                    {
                        new SqlParameter("@Status", newStatus),
                        new SqlParameter("@Id", pu.UserId)
                    };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    pu.Status = newStatus;
                    MessageBox.Show($"Статус пользователя '{pu.Login}' теперь: {newStatus}.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (SelectedEntity is Client cu)
                {
                    var newStatus = cu.Status == "Active" ? "Blocked" : "Active";
                    string query = "UPDATE Clients SET Status = @Status WHERE ClientId = @Id";
                    var prm = new[]
                    {
                        new SqlParameter("@Status", newStatus),
                        new SqlParameter("@Id", cu.ClientId)
                    };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    cu.Status = newStatus;
                    MessageBox.Show($"Статус клиента '{cu.Login}' теперь: {newStatus}.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ChangeRole(object parameter)
        {
            try
            {
                if (SelectedEntity is User pu)
                {
                    var newRole = pu.Role == "Admin" ? "User" : "Admin";
                    string query = "UPDATE Users SET Role = @Role WHERE UserId = @Id";
                    var prm = new[]
                    {
                        new SqlParameter("@Role", newRole),
                        new SqlParameter("@Id", pu.UserId)
                    };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    pu.Role = newRole;
                    MessageBox.Show($"Роль пользователя '{pu.Login}' изменена на {newRole}.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ChangeDiscount(object parameter)
        {
            try
            {
                if (SelectedEntity is Client cu)
                {
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Введите новую скидку (15, 25, 50, 75)", "Изменение скидки", cu.Discount.ToString());
                    if (!decimal.TryParse(input, out decimal discount) || !new[] { 15m, 25m, 50m, 75m }.Contains(discount))
                    {
                        MessageBox.Show("Некорректное значение скидки.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string query = "UPDATE Clients SET Discount = @Discount WHERE ClientId = @Id";
                    var prm = new[]
                    {
                        new SqlParameter("@Discount", discount),
                        new SqlParameter("@Id", cu.ClientId)
                    };
                    DatabaseHelper.ExecuteNonQuery(query, prm);
                    cu.Discount = discount;
                    LoadAllData();
                    MessageBox.Show("Скидка для клиента обновлена.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public abstract class BaseUser
        {
            public int  UserId { get; set; }
            public int ClientId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Login { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
        }

        public class User : BaseUser
        {
            public string Role { get; set; }
        }

        public class Client : BaseUser
        {
            public decimal Discount { get; set; }
        }
    }
}