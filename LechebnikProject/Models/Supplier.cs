namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель поставщика.
    /// </summary>
    public class Supplier
    {
        public int SupplierId { get; set; }         // Уникальный идентификатор
        public string Name { get; set; }            // Название
        public string Country { get; set; }         // Страна
    }
}
