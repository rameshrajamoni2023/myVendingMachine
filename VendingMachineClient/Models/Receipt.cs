using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachineClient.Models
{
    public class Receipt
    {
        public int ReceiptId { get; set; }
        public string? UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LoadedAmount { get; set; }
        public decimal CurrentBalance { get; set; }

        public List<Product>? Items { get; set; }
    }
}
