namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель клиента.
    /// </summary>
    public class Client
    {
        public int ClientId { get; set; }           // Уникальный идентификатор
        public string LastName { get; set; }        // Фамилия
        public string FirstName { get; set; }       // Имя
        public string MiddleName { get; set; }      // Отчество (может быть null)
        public string Login { get; set; }           // Логин
        public string Code { get; set; }            // Код клиента (6 символов)
        public decimal Discount { get; set; }       // Скидка (15%, 25%, 50%, 75%)
        public string Status { get; set; }
    }
}
