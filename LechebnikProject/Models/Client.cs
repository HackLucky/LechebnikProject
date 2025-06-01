namespace LechebnikProject.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Login { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; }
    }
}
