namespace LechebnikProject.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public Medicine Medicine { get; set; }
        public int Quantity { get; set; }
        public bool IsByPrescription { get; set; }
        public int? PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice => Medicine.Price * Quantity * (1 - Discount / 100m);
    }
}
