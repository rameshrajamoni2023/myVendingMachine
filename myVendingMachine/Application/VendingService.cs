using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myVendingMachine.Application.Contracts;
using myVendingMachine.Data;
using myVendingMachine.Helper;
using myVendingMachine.Models;

namespace myVendingMachine.Application
{
    public class VendingService:IVendingService
    {
        private readonly VendingMachineDbContext _dbcontext;

        public VendingService(VendingMachineDbContext dbContext) => _dbcontext = dbContext;

        public async Task<IEnumerable<Product>> GetItems()
        {
            var items = await _dbcontext.Product.ToListAsync();

            return (IEnumerable<Product>)items;
        }

        public async Task<decimal> GetCurrentBalance()
        {
            decimal balance = 0;
            using (_dbcontext)
            {
                var currTxn = await _dbcontext.Transactions.FirstOrDefaultAsync();   //later enhance to pass txnid and support multiple machines

                if (currTxn == null)
                {
                    balance=0;
                    return balance;

                }

                balance = currTxn.Balance;

            }

            return balance;
        }

        public async Task<decimal> LoadMoney(decimal amount)
        {
            decimal balance = 0;

            // Validate the amount
            if (amount <= 0)
            {
                throw new VendingMachineException("Amount should be a positive number.");
            }

            using (_dbcontext)
            {
                var CurrentTxn = await _dbcontext.Transactions.FirstOrDefaultAsync();

                if (CurrentTxn == null)
                {
                    //add new record if none
                    CurrentTxn = new Transactions();
                    _dbcontext.Transactions.Add(CurrentTxn);
                }

                CurrentTxn.TxnDate = DateTime.UtcNow.ToString();
                CurrentTxn.Amount += amount;
                CurrentTxn.Balance += amount;

                balance = CurrentTxn.Balance;

                await _dbcontext.SaveChangesAsync();
            }

            return balance;
           
        }

        public async Task<Product> AddProduct(Product product)
        {
            using (_dbcontext)
            {
                await _dbcontext.Product.AddAsync(product);
                _dbcontext.SaveChanges();
            }
            return product;
        }

        public async Task<int> UpdateProductQuantity(int productId, int quantity)
        {
            var currentQuantity = 0;

            using (_dbcontext)
            {
                var product = await _dbcontext.Product.FindAsync(productId);

                if (product == null)
                {
                    throw new VendingMachineException("product not found.");
                }

                // Update the quantity
                product.Quantity += quantity;
                currentQuantity = product.Quantity;

                _dbcontext.SaveChanges();
            }

            return currentQuantity;
        }



    }
}
