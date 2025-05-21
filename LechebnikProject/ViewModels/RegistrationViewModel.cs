using System;
using System.Data.SqlClient;
using System.Windows;
using LechebnikProject.Helpers;
using LechebnikProject.Models;

namespace LechebnikProject.ViewModels
{
    /// <summary>
    /// ViewModel для окна регистрации нового пользователя.
    /// </summary>
    public class RegistrationViewModel
    {
        // Свойства для полей ввода
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

        /// <summary>
        /// Регистрирует нового пользователя в базе данных.
        /// </summary>
        /// <returns>true, если регистрация успешна</returns>
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
            // Валидация введенных данных
            if (!ValidationHelper.IsValidName(LastName) || !ValidationHelper.IsValidName(FirstName))
            {
                MessageBox.Show("Фамилия и имя должны содержать минимум 2 символа.");
                return false;
            }
            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                MessageBox.Show("Номер телефона должен содержать от 10 до 15 цифр и может начинаться с +.");
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

            // Хеширование пароля
            string hashedPassword = PasswordHasher.HashPassword(Password);

            // SQL-запрос для вставки пользователя
            string query = @"
                INSERT INTO Users (LastName, FirstName, MiddleName, PhoneNumber, Email, Position, PharmacyAddress, Login, PasswordHash, Role, Status)
                VALUES (@LastName, @FirstName, @MiddleName, @PhoneNumber, @Email, @Position, @PharmacyAddress, @Login, @PasswordHash, 'User', 'Active')";

            // Параметры для защиты от SQL-инъекций
            SqlParameter[] parameters2 = {
                new SqlParameter("@LastName", LastName),
                new SqlParameter("@FirstName", FirstName),
                new SqlParameter("@MiddleName", MiddleName ?? (object)DBNull.Value), // Отчество может быть null
                new SqlParameter("@PhoneNumber", PhoneNumber),
                new SqlParameter("@Email", Email),
                new SqlParameter("@Position", Position),
                new SqlParameter("@PharmacyAddress", PharmacyAddress),
                new SqlParameter("@Login", Login),
                new SqlParameter("@PasswordHash", hashedPassword)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters2); // Выполнение запроса
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