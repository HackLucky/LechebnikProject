using System.Text.RegularExpressions;

namespace LechebnikProject.Helpers
{
    /// <summary>
    /// Класс для проверки корректности введенных данных.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Проверяет корректность email.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[\w\.\-]+@[\w\-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Проверяет корректность номера телефона (10-15 цифр, может начинаться с +).
        /// </summary>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
            string pattern = @"^(\+7|8)\d{10}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        /// <summary>
        /// Проверяет корректность имени или фамилии (мин. 2 символа).
        /// </summary>
        public static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2;
        }

        /// <summary>
        /// Проверяет корректность пароля (мин. 8 символов, буквы и цифры).
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
            return Regex.IsMatch(password, @"[A-Za-z]") && Regex.IsMatch(password, @"\d");
        }

        /// <summary>
        /// Проверяет, что строка не пуста.
        /// </summary>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}