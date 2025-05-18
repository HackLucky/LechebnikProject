using System.Security.Cryptography;
using System.Text;

namespace LechebnikProject.Helpers
{
    /// <summary>
    /// Класс для хеширования и проверки паролей.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Хеширует пароль с использованием SHA256.
        /// </summary>
        /// <param name="password">Исходный пароль</param>
        /// <returns>Хеш пароля</returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Преобразование в шестнадцатеричный формат
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Проверяет соответствие введенного пароля сохраненному хешу.
        /// </summary>
        /// <param name="password">Введенный пароль</param>
        /// <param name="hashedPassword">Сохраненный хеш</param>
        /// <returns>true, если пароль верный</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hashedPassword);
        }
    }
}