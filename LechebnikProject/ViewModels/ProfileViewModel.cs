using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ProfileViewModel
    {
        public User CurrentUser => AppContext.CurrentUser;
        public ICommand SaveCommand { get; }
        public ICommand GoBackCommand { get; }

        public ProfileViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void Save(object parameter)
        {
            string query = "UPDATE Users SET LastName = @LastName, FirstName = @FirstName, PhoneNumber = @PhoneNumber, Email = @Email WHERE UserId = @UserId";
            var parameters = new[]
            {
                new SqlParameter("@LastName", CurrentUser.LastName),
                new SqlParameter("@FirstName", CurrentUser.FirstName),
                new SqlParameter("@PhoneNumber", CurrentUser.PhoneNumber),
                new SqlParameter("@Email", CurrentUser.Email),
                new SqlParameter("@UserId", CurrentUser.UserId)
            };
            DatabaseHelper.ExecuteQuery(query, parameters);
            MessageBox.Show("Профиль обновлен!");
        }

        private void GoBack(object parameter)
        {
            var window = new MainMenuWindow();
            window.Show();
            Application.Current.MainWindow = window;
            (Application.Current.MainWindow as Window)?.Close();
        }
    }
}