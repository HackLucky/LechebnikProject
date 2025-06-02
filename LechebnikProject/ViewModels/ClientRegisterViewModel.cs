using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LechebnikProject.ViewModels
{
    public class ClientRegisterViewModel : BaseViewModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Code { get; set; }
        public decimal SelectedDiscount { get; set; } = 15.00m;
        public string CaptchaInput { get; set; }

        private BitmapImage _captchaImage;
        public BitmapImage CaptchaImage
        {
            get => _captchaImage;
            set => SetProperty(ref _captchaImage, value);
        }

        private readonly string _captchaCode;

        public ClientRegisterViewModel()
        {
            var (code, imageBase64) = CaptchaHelper.GenerateCaptcha();
            _captchaCode = code;
            CaptchaImage = ImageHelper.Base64ToBitmapImage(imageBase64);
        }

        public bool Register()
        {
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM Clients WHERE PhoneNumber = @PhoneNumber OR Email = @Email OR Login = @Login";
                var parameters1 = new[]
                {
                    new SqlParameter("@PhoneNumber", PhoneNumber ?? (object)DBNull.Value),
                    new SqlParameter("@Email", Email ?? (object)DBNull.Value),
                    new SqlParameter("@Login", Login)
                };

                int count = (int)DatabaseHelper.ExecuteScalar(checkQuery, parameters1);
                if (count > 0)
                {
                    MessageBox.Show("Клиент или сотрудник с таким телефоном, почтой или логином уже существует.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!ValidationHelper.IsValidName(LastName) || !ValidationHelper.IsValidName(FirstName))
                {
                    MessageBox.Show("Фамилия и имя должны содержать минимум 2 символа.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
                {
                    MessageBox.Show("Введите российский формат номера.\nДолжен начинаться с +7 или 8.\nПосле префикса должно быть 10 цифр.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!ValidationHelper.IsValidEmail(Email))
                {
                    MessageBox.Show("Введите корректный email (например, example@mail.ru).", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrEmpty(Login) || Code.Length != 6)
                {
                    MessageBox.Show("Логин не может быть пустым, а код должен содержать 6 символов.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (CaptchaInput != _captchaCode)
                {
                    MessageBox.Show("Неверный код CAPTCHA.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                string query = "INSERT INTO Clients (LastName, FirstName, MiddleName, Login, Code, Discount, PhoneNumber, Email) VALUES (@LastName, @FirstName, @MiddleName, @Login, @Code, @Discount, @PhoneNumber, @Email)";
                var parameters2 = new[]
                {
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@MiddleName", MiddleName ?? (object)DBNull.Value),
                    new SqlParameter("@Login", Login),
                    new SqlParameter("@Code", Code),
                    new SqlParameter("@Discount", SelectedDiscount),
                    new SqlParameter("@PhoneNumber", PhoneNumber),
                    new SqlParameter("@Email", Email)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters2);
                MessageBox.Show($"Клиент успешно зарегистрирован! Код клиента: {Code}", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowManager.ShowWindow<ClientLoginWindow>();
                return true;
            }
            catch { MessageBox.Show("Ошибка регистрации. Возможно, логин уже занят.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
    }
}