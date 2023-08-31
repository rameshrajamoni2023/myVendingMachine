using System.ComponentModel.DataAnnotations;

namespace myVendingMachine.Models
{

    public class TransactionDetails
    {
        [Key]
        public int TxnDetailId { get; set; }
        public int TxnId { get; set; }
        public int ProductId { get; set; }

    }
}
