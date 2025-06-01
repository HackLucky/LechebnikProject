using System;

namespace LechebnikProject.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public bool DiscountApplied { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

}
