using System;

namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель заказа.
    /// </summary>
    public class Order
    {
        public int OrderId { get; set; }            // Уникальный идентификатор
        public int UserId { get; set; }             // Идентификатор пользователя
        public int? ClientId { get; set; }          // Идентификатор клиента (может быть null)
        public DateTime OrderDate { get; set; }     // Дата и время заказа
        public string PaymentMethod { get; set; }   // Способ оплаты
        public decimal TotalAmount { get; set; }    // Общая сумма
        public bool DiscountApplied { get; set; }   // Применена ли скидка
        public decimal? DiscountPercentage { get; set; } // Процент скидки
    }

}
