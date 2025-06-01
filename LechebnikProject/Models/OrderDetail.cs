namespace LechebnikProject.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public bool DiscountApplied { get; set; }
        public decimal? DiscountedPricePerUnit { get; set; }
    }
}
