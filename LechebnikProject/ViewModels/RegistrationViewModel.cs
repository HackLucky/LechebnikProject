using LechebnikProject.Helpers;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LechebnikProject.ViewModels
{
    public class RegistrationViewModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string PharmacyAddress { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CaptchaInput { get; set; }

        private BitmapImage _captchaImage;
        public BitmapImage CaptchaImage
        {
            get => _captchaImage;
            set => _captchaImage = value; // Здесь нет INotifyPropertyChanged, так как это не MVVM-класс
        }

        private readonly string _captchaCode;

        public RegistrationViewModel()
        {
            var (code, imageBase64) = CaptchaHelper.GenerateCaptcha();
            _captchaCode = code;
            CaptchaImage = ImageHelper.Base64ToBitmapImage(imageBase64);
        }

        public bool Register()
        {
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE PhoneNumber = @PhoneNumber OR Email = @Email OR Login = @Login";
            var parameters1 = new[]
            {
                new SqlParameter("@PhoneNumber", PhoneNumber),
                new SqlParameter("@Email", Email),
                new SqlParameter("@Login", Login)
            };
            int count = (int)DatabaseHelper.ExecuteScalar(checkQuery, parameters1);
            if (count > 0)
            {
                MessageBox.Show("Пользователь с таким телефоном, почтой или логином уже существует.");
                return false;
            }
            if (!ValidationHelper.IsValidName(LastName) || !ValidationHelper.IsValidName(FirstName))
            {
                MessageBox.Show("Фамилия и имя должны содержать минимум 2 символа.");
                return false;
            }
            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                MessageBox.Show("Введите российский формат номера.\nДолжен начинаться с +7 или 8.\nПосле префикса должно быть 10 цифр.");
                return false;
            }
            if (!ValidationHelper.IsValidEmail(Email))
            {
                MessageBox.Show("Введите корректный email (например, example@mail.ru).");
                return false;
            }
            if (!ValidationHelper.IsNotEmpty(Position) || !ValidationHelper.IsNotEmpty(PharmacyAddress))
            {
                MessageBox.Show("Должность и адрес аптеки не могут быть пустыми.");
                return false;
            }
            if (!ValidationHelper.IsNotEmpty(Login))
            {
                MessageBox.Show("Логин не может быть пустым.");
                return false;
            }
            if (!ValidationHelper.IsValidPassword(Password))
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов, включая буквы и цифры.");
                return false;
            }
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return false;
            }
            if (CaptchaInput != _captchaCode)
            {
                MessageBox.Show("Неверный код CAPTCHA.");
                return false;
            }

            string hashedPassword = PasswordHasher.HashPassword(Password);
            string query = @"
                INSERT INTO Users (LastName, FirstName, MiddleName, PhoneNumber, Email, Position, PharmacyAddress, Login, PasswordHash, Role, Status)
                VALUES (@LastName, @FirstName, @MiddleName, @PhoneNumber, @Email, @Position, @PharmacyAddress, @Login, @PasswordHash, 'User', 'Active')";
            SqlParameter[] parameters2 = {
                new SqlParameter("@LastName", LastName),
                new SqlParameter("@FirstName", FirstName),
                new SqlParameter("@MiddleName", MiddleName ?? (object)DBNull.Value),
                new SqlParameter("@PhoneNumber", PhoneNumber),
                new SqlParameter("@Email", Email),
                new SqlParameter("@Position", Position),
                new SqlParameter("@PharmacyAddress", PharmacyAddress),
                new SqlParameter("@Login", Login),
                new SqlParameter("@PasswordHash", hashedPassword)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters2);
                MessageBox.Show("Регистрация прошла успешно!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Ошибка при регистрации пользователя.", ex);
                MessageBox.Show("Ошибка при регистрации. Возможно, логин уже занят.");
                return false;
            }
        }
    }
}