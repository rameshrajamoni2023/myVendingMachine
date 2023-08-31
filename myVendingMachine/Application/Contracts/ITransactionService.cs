using Microsoft.AspNetCore.Mvc;
using myVendingMachine.Models;

namespace myVendingMachine.Application.Contracts
{
    public interface ITransactionService
    {
        Task<decimal> PurchaseItem(int itemId);
        Task<decimal> ReturnMoney();

        Task<Receipt> Receipt();

        Task<string> ResetDetails();
    }
}
