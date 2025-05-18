namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель пользователя (фармацевта или администратора).
    /// </summary>
    public class User
    {
        public int UserId { get; set; }             // Уникальный идентификатор
        public string LastName { get; set; }        // Фамилия
        public string FirstName { get; set; }       // Имя
        public string MiddleName { get; set; }      // Отчество (может быть null)
        public string PhoneNumber { get; set; }     // Номер телефона
        public string Email { get; set; }           // Электронная почта
        public string Position { get; set; }        // Должность
        public string PharmacyAddress { get; set; } // Адрес аптеки
        public string Login { get; set; }           // Логин
        public string PasswordHash { get; set; }    // Хеш пароля
        public string Role { get; set; }            // Роль (User/Admin)
        public string Status { get; set; }          // Статус (Active/Blocked)
    }
}