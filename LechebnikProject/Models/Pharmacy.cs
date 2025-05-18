namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель аптеки.
    /// </summary>
    public class Pharmacy
    {
        public int PharmacyId { get; set; }         // Уникальный идентификатор
        public string Address { get; set; }         // Адрес
        public string PhoneNumber { get; set; }     // Номер телефона
    }
}
