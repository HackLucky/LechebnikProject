using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using Microsoft.VisualBasic;
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
        public ObservableCollection<object> AllEntities { get; set; } = new ObservableCollection<object>();
        public object SelectedEntity { get; set; }

        public ICommand ToggleBlockCommand { get; }
        public ICommand ChangeRoleCommand { get; }
        public ICommand ChangeDiscountCommand { get; }
        public ICommand GoBackCommand { get; }

        public ManageUsersViewModel()
        {
            LoadAllData();
            ToggleBlockCommand = new RelayCommand(ToggleBlock, o => SelectedEntity != null);
            ChangeRoleCommand = new RelayCommand(ChangeRole, o => SelectedEntity is User);
            ChangeDiscountCommand = new RelayCommand(ChangeDiscount, o => SelectedEntity is Client);
            GoBackCommand = new RelayCommand(_ => WindowManager.ShowWindow<AdminPanelWindow>());
        }

        private void LoadAllData()
        {
            AllEntities.Clear();
            LoadUsers();
            LoadClients();
        }

        private void LoadUsers()
        {
            string query = "SELECT * FROM Users";
            DataTable table = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                AllEntities.Add(new User
                {
                    UserId = (int)row["UserId"],
                    LastName = row["LastName"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Email = row["Email"].ToString(),
                    Position = row["Position"].ToString(),
                    PharmacyAddress = row["PharmacyAddress"].ToString(),
                    Login = row["Login"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    Role = row["Role"].ToString(),
                    Status = row["Status"].ToString()
                });
            }
        }

        private void LoadClients()
        {
            string query = "SELECT * FROM Clients";
            DataTable table = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                AllEntities.Add(new Client
                {
                    ClientId = (int)row["ClientId"],
                    LastName = row["LastName"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    Login = row["Login"].ToString(),
                    Code = row["Code"].ToString(),
                    Discount = (decimal)row["Discount"],
                    Status = row["Status"].ToString()
                });
            }
        }

        private void ToggleBlock(object obj)
        {
            if (SelectedEntity is User user)
            {
                string newStatus = user.Status == "Active" ? "Blocked" : "Active";
                string query = "UPDATE Users SET Status = @Status WHERE UserId = @Id";
                var prm = new[]
                {
                    new SqlParameter("@Status", newStatus),
                    new SqlParameter("@Id", user.UserId)
                };
                DatabaseHelper.ExecuteNonQuery(query, prm);
                user.Status = newStatus;
                MessageBox.Show($"Пользователь {user.Login} теперь: {newStatus}.", "Информация");
            }
            else if (SelectedEntity is Client client)
            {
                string newStatus = client.Status == "Active" ? "Blocked" : "Active";
                string query = "UPDATE Clients SET Status = @Status WHERE ClientId = @Id";
                var prm = new[]
                {
                    new SqlParameter("@Status", newStatus),
                    new SqlParameter("@Id", client.ClientId)
                };
                DatabaseHelper.ExecuteNonQuery(query, prm);
                client.Status = newStatus;
                MessageBox.Show($"Клиент {client.Login} теперь: {newStatus}.", "Информация");
            }
        }

        private void ChangeRole(object obj)
        {
            if (!(SelectedEntity is User user)) return;
            string newRole = user.Role == "Admin" ? "User" : "Admin";
            string query = "UPDATE Users SET Role = @Role WHERE UserId = @Id";
            var prm = new[]
            {
                new SqlParameter("@Role", newRole),
                new SqlParameter("@Id", user.UserId)
            };
            DatabaseHelper.ExecuteNonQuery(query, prm);
            user.Role = newRole;
            MessageBox.Show($"Роль пользователя {user.Login} изменена на {newRole}.", "Информация");
        }

        private void ChangeDiscount(object obj)
        {
            if (!(SelectedEntity is Client client)) return;

            string input = Interaction.InputBox("Введите новую скидку (15, 25, 50, 75)", "Изменение скидки", client.Discount.ToString());
            if (!decimal.TryParse(input, out decimal discount) || !new[] { 15m, 25m, 50m, 75m }.Contains(discount))
            {
                MessageBox.Show("Некорректное значение скидки.", "Ошибка");
                return;
            }
            string query = "UPDATE Clients SET Discount = @Discount WHERE ClientId = @Id";
            var prm = new[]
            {
                new SqlParameter("@Discount", discount),
                new SqlParameter("@Id", client.ClientId)
            };
            DatabaseHelper.ExecuteNonQuery(query, prm);
            client.Discount = discount;
            MessageBox.Show("Скидка обновлена.", "Информация");
        }
    }
}