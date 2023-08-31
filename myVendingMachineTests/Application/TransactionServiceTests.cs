using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using myVendingMachine.Data;
using myVendingMachine.Models;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using myVendingMachine.Helper;

namespace myVendingMachine.Application.Tests
{
    [TestClass()]
    public class TransactionServiceTests
    {
        private DbContextOptions<TransactionDetailsDbContext> _options;
        private TransactionDetailsDbContext _context;
        private TransactionService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            // Since we are using same primary key for some test cases we need to create new db each time in memory
            var dbName = "Test_" + System.Guid.NewGuid();

            _options = new DbContextOptionsBuilder<TransactionDetailsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new TransactionDetailsDbContext(_options);

            _service = new TransactionService(_context);


        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task PurchaseItem_SuccessfulPurchase()
        {
            // Arrange
            // Add a product and a transaction
            _context.Product.Add(new Product
            {
                Id = 1,
                Name = "Product A",
                Rate = 2.0M,
                Quantity = 10

            });
            
            _context.Transactions.Add(new Transactions
            {
                TxnId = 1,
                Amount = 20.0M,
                Balance = 10.0M
            });

            _context.SaveChanges();
      
            // Act
            var balance = await _service.PurchaseItem(1);    //pass ProductId

            // Assert
            Assert.AreEqual(8.0M, balance); // Expected balance after purchase 10-2 = 8
        }

        [TestMethod]
        [ExpectedException(typeof(VendingMachineException), "Item not found")]
        public async Task PurchaseItem_ItemNotFound()
        {
            // Arrange
            // Add a product and a transaction
            _context.Product.Add(new Product
            {
                Id = 1,
                Name = "Product A",
                Rate = 1.99M,
                Quantity = 10

            });

            _context.Transactions.Add(new Transactions
            {
                TxnId = 1,
                Amount = 20.0M,
                Balance = 10.0M
            });

            _context.SaveChanges();
            

            // Act
            var balance = await _service.PurchaseItem(2); // pass non-existing id

            // Assert
            // Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(VendingMachineException), "Item is out of stock")]
        public async Task PurchaseItem_ItemOutOfStock()
        {
            // Arrange
            // Add a product and a transaction
            _context.Product.Add(new Product
            {
                Id = 1,
                Name = "Product A",
                Rate = 2.0M,
                Quantity = 0                    //no quantity available

            });

            _context.Transactions.Add(new Transactions
            {
                TxnId = 1,
                Amount = 20.0M,
                Balance = 10.0M
            });

            _context.SaveChanges();
         
            // Act
            await _service.PurchaseItem(1);

            // Assert
            // Exception expected, so no additional assertions
        }

        [TestMethod]
        [ExpectedException(typeof(VendingMachineException), "Unable to Read Funds")]
        public async Task PurchaseItem_UnableToReadFunds()
        {
            // Arrange
            // Add a product but don't add transaction so that it throws exception
     
            _context.Product.Add(new Product
            {
                Id = 1,
                Quantity = 10,
                Rate = 2.0M
            });
            
            _context.SaveChanges();
          
            // Act
            await _service.PurchaseItem(1);

            // Assert
            // Exception expected, so no additional assertions
        }

        [TestMethod]
        [ExpectedException(typeof(VendingMachineException), "Insufficient Funds")]
        public async Task PurchaseItem_InsufficientFunds()
            {
                // Arrange
                // Add a product and a transaction
                _context.Product.Add(new Product
                {
                    Id = 1,
                    Quantity = 10,
                    Rate = 20.0M                //put a rate to make sure you get insufficient funds  
                });
                
                _context.Transactions.Add(new Transactions
                {
                    TxnId = 1,
                    TxnDate = DateTime.Now.ToString(),
                    Amount = 15.0M,           //Amt Required=20*10=200, but we set only 15
                    Balance = 10.0M
                });
                
                _context.SaveChanges();
                ;

                // Act
                await _service.PurchaseItem(1);

                // Assert
                // Exception expected, so no additional assertions
            }
    }
}