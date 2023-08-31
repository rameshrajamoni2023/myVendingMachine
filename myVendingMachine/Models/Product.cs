namespace myVendingMachine.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Rate { get; set; }
        public int Quantity { get; set; }

    }
}
