namespace LechebnikProject.Models
{
    public class CartItem
    {
        public Medicine Medicine { get; set; }
        public int Quantity { get; set; }
        public bool IsByPrescription { get; set; }
        public Prescription Prescription { get; set; }
    }
}