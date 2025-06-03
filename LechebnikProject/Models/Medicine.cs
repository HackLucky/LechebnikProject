namespace LechebnikProject.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Form { get; set; }
        public string WeightVolume { get; set; }
        public string SerialNumber { get; set; }
        public string Usage { get; set; }
        public string ActiveIngredient { get; set; }
        public string ApplicationMethod { get; set; }
        public string AggregateState { get; set; }
        public string Type { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerCountry { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCountry { get; set; }
        public int StockQuantity { get; set; }
        public bool RequiresPrescription { get; set; }
        public decimal Price { get; set; }
    }
}
