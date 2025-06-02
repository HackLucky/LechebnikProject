namespace LechebnikProject.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public Medicine Medicine { get; set; }
        public int Quantity { get; set; }
        public bool IsByPrescription { get; set; }
        public int? PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public decimal Discount { get; set; } // Добавляем скидку (0, 15, 25, 50, 75 или 50/100 для рецептов)
        public decimal TotalPrice => Medicine.Price * Quantity * (1 - Discount / 100m); // Учитываем скидку
    }
}