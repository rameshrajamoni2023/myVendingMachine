using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using myVendingMachine.Models;
using myVendingMachine.Data;


namespace myVendingMachine.Application.Tests
{
    [TestClass()]
    public class VendingServiceTests
    {
        
        private DbContextOptions<VendingMachineDbContext> _options;
        private VendingMachineDbContext _context;
        private VendingService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            // Since we are using same primary key for some test cases we need to create new db each time in memory
            var dbName = "Test_" + System.Guid.NewGuid();

            _options = new DbContextOptionsBuilder<VendingMachineDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new VendingMachineDbContext(_options);

            _service = new VendingService(_context);


        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetItems_ReturnsListOfProducts()
        {
            // Arrange
            _context.Product.AddRange(
                new Product { Id = 1, Name = "Product A", Rate = 2.99M, Quantity=10 },
                new Product { Id = 2, Name = "Product B", Rate = 1.49M, Quantity = 20 }
            );

            _context.SaveChanges();

            // Act
            var products = await _service.GetItems();

            // Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(2, products.Count());

            Assert.AreEqual("Product A", products.First().Name);

        }

        [TestMethod]
        public async Task GetBalance_ReturnsLatestBalance()
        {
            // Arrange
            decimal expectedBalance = 10.0m;

            // Populate the in-memory database with a sample transaction
            _context.Transactions.Add(new Transactions 
                    { TxnId=1,TxnDate=DateTime.UtcNow.ToString(),Amount = 5, Balance = 10.0M });
            
            _context.SaveChanges();

           
            // Act
            var balance = await _service.GetCurrentBalance();

            // Assert
            Assert.IsNotNull(balance);
            Assert.AreEqual(expectedBalance,balance);

        }

        //[TODO] unit tests for remaining methods

    }
}
