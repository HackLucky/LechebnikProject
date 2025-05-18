namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель деталей заказа.
    /// </summary>
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }      // Уникальный идентификатор
        public int OrderId { get; set; }            // Идентификатор заказа
        public int MedicineId { get; set; }         // Идентификатор препарата
        public int Quantity { get; set; }           // Количество
        public decimal PricePerUnit { get; set; }   // Цена за единицу
        public bool DiscountApplied { get; set; }   // Применена ли скидка
        public decimal? DiscountedPricePerUnit { get; set; } // Цена с учетом скидки
    }
}
