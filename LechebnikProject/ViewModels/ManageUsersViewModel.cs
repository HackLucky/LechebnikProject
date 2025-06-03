using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using Microsoft.VisualBasic;
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
        public ObservableCollection<object> AllEntities { get; set; }
        public object SelectedEntity { get; set; }

        public ICommand DelUserCommand { get; }
        public ICommand ChangeRoleCommand { get; }
        public ICommand ChangeDiscountCommand { get; }
        public ICommand ToggleBlockCommand { get; }
        public ICommand GoBackCommand { get; }

        public ManageUsersViewModel()
        {
            AllEntities = new ObservableCollection<object>();
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
        }

        private void LoadUsers()
        {
            try
            {
                string query = "SELECT UserId, LastName, FirstName, MiddleName, Login, Role, Status FROM Users";
                DataTable table = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in table.Rows)
                {
                    AllEntities.Add(new
                    {
                        Type = "User",
                        Id = row["UserId"],
                        LastName = row["LastName"]?.ToString() ?? "",
                        FirstName = row["FirstName"]?.ToString() ?? "",
                        MiddleName = row["MiddleName"]?.ToString(),
                        Login = row["Login"]?.ToString() ?? "",
                        Role = row["Role"]?.ToString() ?? "",
                        Status = row["Status"]?.ToString() ?? "Active",
                        Discount = (decimal?)null
                    });
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
                    AllEntities.Add(new
                    {
                        Type = "Client",
                        Id = row["ClientId"],
                        LastName = row["LastName"]?.ToString() ?? "",
                        FirstName = row["FirstName"]?.ToString() ?? "",
                        MiddleName = row["MiddleName"]?.ToString(),
                        Login = row["Login"]?.ToString() ?? "",
                        Role = (string)null,
                        Status = row["Status"]?.ToString() ?? "Active",
                        Discount = row["Discount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["Discount"]) : null
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void DeleteEntity(object parameter)
        {
            try
            {
                if (SelectedEntity == null) return;
                string type = (string)SelectedEntity.GetType().GetProperty("Type").GetValue(SelectedEntity);
                int id = (int)SelectedEntity.GetType().GetProperty("Id").GetValue(SelectedEntity);

                string query = type == "User"
                    ? "DELETE FROM Users WHERE UserId = @Id"
                    : "DELETE FROM Clients WHERE ClientId = @Id";
                var prm = new[] { new SqlParameter("@Id", id) };
                DatabaseHelper.ExecuteNonQuery(query, prm);
                MessageBox.Show($"{type} удалён.", "Информация");
                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ToggleBlock(object parameter)
        {
            try
            {
                if (SelectedEntity == null) return;
                string type = (string)SelectedEntity.GetType().GetProperty("Type").GetValue(SelectedEntity);
                int id = (int)SelectedEntity.GetType().GetProperty("Id").GetValue(SelectedEntity);
                string currentStatus = (string)SelectedEntity.GetType().GetProperty("Status").GetValue(SelectedEntity);
                string newStatus = currentStatus == "Active" ? "Blocked" : "Active";

                string query = type == "User"
                    ? "UPDATE Users SET Status = @Status WHERE UserId = @Id"
                    : "UPDATE Clients SET Status = @Status WHERE ClientId = @Id";
                var prm = new[]
                {
                    new SqlParameter("@Status", newStatus),
                    new SqlParameter("@Id", id)
                };
                DatabaseHelper.ExecuteNonQuery(query, prm);
                MessageBox.Show($"{type} {SelectedEntity.GetType().GetProperty("Login").GetValue(SelectedEntity)} теперь: {newStatus}.", "Информация");
                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ChangeRole(object obj)
        {
            try
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
                LoadAllData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ChangeDiscount(object obj)
        {
            try
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
                LoadAllData();
                MessageBox.Show("Скидка обновлена.", "Информация");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}