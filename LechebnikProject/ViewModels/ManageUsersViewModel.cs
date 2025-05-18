using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private DataTable _users;
        private object _selectedUser;

        public DataTable Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public object SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand BlockUserCommand { get; }
        public ICommand GoBackCommand { get; }

        public ManageUsersViewModel()
        {
            LoadUsers();
            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser, CanEditOrBlock);
            BlockUserCommand = new RelayCommand(BlockUser, CanEditOrBlock);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadUsers()
        {
            string query = "SELECT UserId, LastName, FirstName, PhoneNumber, Email, Position, Role, Status FROM Users";
            Users = DatabaseHelper.ExecuteQuery(query);
        }

        private void AddUser(object parameter)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
            LoadUsers();
        }

        private void EditUser(object parameter)
        {
            if (SelectedUser is DataRowView row)
            {
                var user = new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    LastName = row["LastName"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Email = row["Email"].ToString(),
                    Position = row["Position"].ToString(),
                    Role = row["Role"].ToString(),
                    Status = row["Status"].ToString()
                };

                string query = "UPDATE Users SET LastName = @LastName, FirstName = @FirstName, PhoneNumber = @PhoneNumber, Email = @Email, Position = @Position, Role = @Role WHERE UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@LastName", user.LastName),
                    new SqlParameter("@FirstName", user.FirstName),
                    new SqlParameter("@PhoneNumber", user.PhoneNumber),
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@Position", user.Position),
                    new SqlParameter("@Role", user.Role),
                    new SqlParameter("@UserId", user.UserId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadUsers();
            }
        }

        private void BlockUser(object parameter)
        {
            if (SelectedUser is DataRowView row)
            {
                int userId = Convert.ToInt32(row["UserId"]);
                string status = row["Status"].ToString() == "Active" ? "Blocked" : "Active";
                string query = "UPDATE Users SET Status = @Status WHERE UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@Status", status),
                    new SqlParameter("@UserId", userId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadUsers();
            }
        }

        private bool CanEditOrBlock(object parameter) => SelectedUser != null;

        private void GoBack(object parameter)
        {
            var adminPanelWindow = new AdminPanelWindow();
            adminPanelWindow.Show();
            Application.Current.Windows.OfType<ManageUsersWindow>().FirstOrDefault()?.Close();
        }
    }
}