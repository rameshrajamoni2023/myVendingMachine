using System.ComponentModel.DataAnnotations;

namespace myVendingMachine.Models
{
    public class Transactions
    {
        [Key]
        public int TxnId { get; set; }
        public string TxnDate { get; set; } = DateTime.Now.ToString();
        public string Action { get; set; } = "NEW";
        public string User { get; set; } = "MACID001";
        public Decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; } = "SUCESS";



    }



}
