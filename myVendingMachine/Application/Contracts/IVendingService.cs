using Microsoft.AspNetCore.Mvc;
using myVendingMachine.Models;

namespace myVendingMachine.Application.Contracts
{
    public interface IVendingService    
    {
       Task<IEnumerable<Product>> GetItems();
       Task<decimal> GetCurrentBalance();
       Task<decimal> LoadMoney(decimal amount);
       Task<Product> AddProduct(Product product);
       Task<int> UpdateProductQuantity(int productId, int quantity);
    }
}
