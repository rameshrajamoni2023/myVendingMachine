using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myVendingMachine.Application.Contracts;
using myVendingMachine.Data;
using myVendingMachine.Helper;
using myVendingMachine.Models;

namespace myVendingMachine.Application
{
    public class TransactionService:ITransactionService
    {
        private readonly TransactionDetailsDbContext _transactionDetailsDbContext;

        public TransactionService(TransactionDetailsDbContext transactionDetailsDbContext) => _transactionDetailsDbContext = transactionDetailsDbContext;

        public async Task<decimal> PurchaseItem(int itemId)
        {

            decimal balance = 0;

            using (_transactionDetailsDbContext)
            {
                var item = await _transactionDetailsDbContext.Product.FindAsync(itemId);
                var txn = await _transactionDetailsDbContext.Transactions.FirstOrDefaultAsync();

                if (item == null)
                {
                    throw new VendingMachineException("Item not found");
                }

                if (item.Quantity <= 0)
                {
                    throw new VendingMachineException("Item is out of stock");
                }

                if (txn == null)
                {
                    throw new VendingMachineException("Unable to Read Funds");
                }

                item.Quantity--;

                txn.Balance = txn.Balance - item.Rate;
                balance = txn.Balance;

                if(balance<0)
                {
                    throw new VendingMachineException("InSufficient Funds");
                }


                //always add new record for each purchase
                var txnDetails = new TransactionDetails();

                txnDetails.TxnId = txn.TxnId;
                txnDetails.TxnDetailId = GetLastTransactionDetailsID() + 1;
                txnDetails.ProductId = item.Id;

                _transactionDetailsDbContext.TransactionDetails.Add(txnDetails);

                await _transactionDetailsDbContext.SaveChangesAsync();

                return balance;

            }
        }

        public async Task<decimal> ReturnMoney()
        {
            decimal balance = 0;

            using (_transactionDetailsDbContext)
            {
                var CurrentTxn = await _transactionDetailsDbContext.Transactions.FirstOrDefaultAsync();

                if (CurrentTxn == null)
                {
                    throw new Exception("Unable to Fetch Fund");
                }

                balance = CurrentTxn.Balance;

                CurrentTxn.TxnDate = DateTime.UtcNow.ToString();
                CurrentTxn.Amount = 0;       //[TODO] DB side if want to support concurrent users, current assumption one user
                CurrentTxn.Balance = 0;

                //reset txndetails also
                var entitiesToDelete = _transactionDetailsDbContext.TransactionDetails
                                    .Where(e => e.TxnId == CurrentTxn.TxnId)
                                    .ToList();

                if (entitiesToDelete.Any())
                {
                    _transactionDetailsDbContext.TransactionDetails.RemoveRange(entitiesToDelete);
                }

                await _transactionDetailsDbContext.SaveChangesAsync();

                return balance;
            }
        }

        public async Task<Receipt> Receipt()
        {

            Receipt receipt = new Receipt();

            using (_transactionDetailsDbContext)
            {
                var CurrentTxn = await _transactionDetailsDbContext.Transactions.FirstOrDefaultAsync();

                if (CurrentTxn == null)
                {
                    throw new VendingMachineException("Unable to Fetch Receipt Details");
                }

                receipt.PurchaseDate = DateTime.UtcNow;
                receipt.ReceiptId = Utils.GetReceiptId();
                receipt.CurrentBalance = CurrentTxn.Balance;
                receipt.LoadedAmount = CurrentTxn.Amount;

                //read list of items purchase
                var transactionDetails = _transactionDetailsDbContext.TransactionDetails
                                           .Where(td => td.TxnId == CurrentTxn.TxnId)
                                           .GroupBy(td => td.ProductId)
                                            .Select(group => new
                                            {
                                                ProductId = group.Key,
                                                Count = group.Count()
                                            })
                                            .ToList();


                //var transactionDetails = _transactionDetailsDbContext.TransactionDetails
                //                            .Where(td => td.TxnId == CurrentTxn.TxnId)
                //                            .ToList();

                var Products = _transactionDetailsDbContext.Product.ToList();

                var item = new Product();
                receipt.Items = new List<Product>();

                if (transactionDetails.Any())
                {
                    Console.WriteLine($"Purchase Items found for Transaction Id {0} ", CurrentTxn.TxnId);

                    foreach (var detail in transactionDetails)
                    {
                        //we will reuse the products data and override the Quantity with the Quantity purchased here
                        item = Products.FirstOrDefault(product => product.Id == detail.ProductId);
                        if (item != null)
                        {
                            item.Quantity = detail.Count;
                            receipt.Items.Add(item);
                            receipt.TotalAmount += item.Rate * item.Quantity;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"No Items found for Transaction Id {0}", CurrentTxn.TxnId);
                }
            }

            return receipt;
        }

        public async Task<string> ResetDetails()
        {
            string result = string.Empty;

            using (_transactionDetailsDbContext)
            {
                var CurrentTxn = await _transactionDetailsDbContext.Transactions.FirstOrDefaultAsync();

                if (CurrentTxn == null)
                {
                    throw new VendingMachineException("Unable to reset data!");
                }

                CurrentTxn.Amount = 0;
                CurrentTxn.Balance = 0;


                var entitiesToDelete = _transactionDetailsDbContext.TransactionDetails
                                        .Where(e => e.TxnId == CurrentTxn.TxnId)
                                        .ToList();

                if (entitiesToDelete.Any())
                {
                    _transactionDetailsDbContext.TransactionDetails.RemoveRange(entitiesToDelete);

                    result = $"{entitiesToDelete.Count} transactions reset successfully.";
                    Console.WriteLine($"{entitiesToDelete.Count} rows deleted for Txn ID {CurrentTxn.TxnId}.");
                }
                else
                {
                    result = $"no data found to reset.";
                    Console.WriteLine($"No rows found for Txn ID {CurrentTxn.TxnId} to delete.");
                }

                _transactionDetailsDbContext.SaveChanges();
            }

            return result;
        }

        private int GetLastTransactionID()
        {
            int lastTransactionID = _transactionDetailsDbContext.Transactions
                .OrderByDescending(t => t.TxnId)
                .Select(t => t.TxnId)
                .FirstOrDefault();

            if (lastTransactionID <= 0) { lastTransactionID = 0; }

            return lastTransactionID;
        }

        private int GetLastTransactionDetailsID()
        {
            int lastTransactionID = _transactionDetailsDbContext.TransactionDetails
                .OrderByDescending(t => t.TxnDetailId)
                .Select(t => t.TxnDetailId)
                .FirstOrDefault();

            if (lastTransactionID <= 0) { lastTransactionID = 0; }

            return lastTransactionID;
        }



    }
}
