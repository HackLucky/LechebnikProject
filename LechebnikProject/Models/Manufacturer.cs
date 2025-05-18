namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель производителя.
    /// </summary>
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }     // Уникальный идентификатор
        public string Name { get; set; }            // Название
        public string Country { get; set; }         // Страна
    }
}
