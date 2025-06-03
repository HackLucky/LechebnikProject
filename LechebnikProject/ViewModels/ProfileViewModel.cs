using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
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
        public ICommand ExitProfile {  get; }

        public ProfileViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
            ExitProfile = new RelayCommand(o => WindowManager.ShowWindow<LoginWindow>());
        }

        private void Save(object parameter)
        {
            if (!ValidationHelper.IsValidName(CurrentUser.LastName) || !ValidationHelper.IsValidName(CurrentUser.FirstName))
            {
                MessageBox.Show("Фамилия и имя должны содержать минимум 2 символа.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidationHelper.IsValidPhoneNumber(CurrentUser.PhoneNumber))
            {
                MessageBox.Show("Введите российский формат номера.\nДолжен начинаться с +7 или 8.\nПосле префикса должно быть 10 цифр.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidationHelper.IsValidEmail(CurrentUser.Email))
            {
                MessageBox.Show("Введите корректный email (например, example@mail.ru).", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidationHelper.IsNotEmpty(CurrentUser.Position) || !ValidationHelper.IsNotEmpty(CurrentUser.PharmacyAddress))
            {
                MessageBox.Show("Должность и адрес аптеки не могут быть пустыми.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
            try
            {
                DatabaseHelper.ExecuteQuery(query, parameters);
                MessageBox.Show("Профиль обновлен!", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError("Ошибка при обновлении профиля.", ex);
                MessageBox.Show("Ошибка при обновлении профиля.", "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}