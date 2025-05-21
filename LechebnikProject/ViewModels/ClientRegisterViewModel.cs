using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using System;
using System.Data.SqlClient;
using System.Windows;

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
        public decimal SelectedDiscount { get; set; } = 15.00m; // По умолчанию 15%
        public string CaptchaInput { get; set; }
        public string CaptchaImage { get; set; }
        private readonly string _captchaCode;

        public ClientRegisterViewModel()
        {
            var (code, image) = CaptchaHelper.GenerateCaptcha();
            _captchaCode = code;
            CaptchaImage = $"data:image/png;base64,{image}";
        }

        public bool Register()
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
                MessageBox.Show("Клиент с таким телефоном, почтой или логином уже существует.");
                return false;
            }
            if (!ValidationHelper.IsValidName(LastName) || !ValidationHelper.IsValidName(FirstName))
            {
                MessageBox.Show("Фамилия и имя должны содержать минимум 2 символа.");
                return false;
            }
            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                MessageBox.Show("Номер телефона должен содержать от 10 до 15 цифр.");
                return false;
            }
            if (!ValidationHelper.IsValidEmail(Email))
            {
                MessageBox.Show("Введите корректный email.");
                return false;
            }
            if (string.IsNullOrEmpty(Login) || Code.Length != 6)
            {
                MessageBox.Show("Логин не может быть пустым, а код должен содержать 6 символов.");
                return false;
            }
            if (CaptchaInput != _captchaCode)
            {
                MessageBox.Show("Неверный код CAPTCHA.");
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

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters2);
                MessageBox.Show($"Клиент успешно зарегистрирован! Код клиента: {Code}");
                return true;
            }
            catch
            {
                MessageBox.Show("Ошибка регистрации. Возможно, логин уже занят.");
                return false;
            }
        }
    }
}