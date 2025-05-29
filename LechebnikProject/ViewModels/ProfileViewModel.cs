using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ProfileViewModel
    {
        public User CurrentUser => AppContext.CurrentUser;
        public ICommand SaveCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ExitProfile {  get; }

        public ProfileViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
            ExitProfile = new RelayCommand(o => WindowManager.ShowWindow<LoginWindow>());
        }

        private void Save(object parameter)
        {
            string query = "UPDATE Users SET LastName = @LastName, FirstName = @FirstName, MiddleName = @MiddleName, PhoneNumber = @PhoneNumber, Email = @Email, Position = @Position, PharmacyAddress = @PharmacyAddress WHERE UserId = @UserId";
            var parameters = new[]
            {
                new SqlParameter("@LastName", CurrentUser.LastName),
                new SqlParameter("@FirstName", CurrentUser.FirstName),
                new SqlParameter("@MiddleName", CurrentUser.MiddleName),
                new SqlParameter("@PhoneNumber", CurrentUser.PhoneNumber),
                new SqlParameter("@Email", CurrentUser.Email),
                new SqlParameter("@Position", CurrentUser.Position),
                new SqlParameter("@PharmacyAddress", CurrentUser.PharmacyAddress),
                new SqlParameter("@UserId", CurrentUser.UserId)
            };
            DatabaseHelper.ExecuteQuery(query, parameters);
            MessageBox.Show("Профиль обновлен!");
        }
    }
}