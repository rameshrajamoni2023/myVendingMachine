using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using myVendingMachine.Application.Contracts;
using myVendingMachine.Models;
using myVendingMachine.Controllers;
using System.Web;
using Microsoft.EntityFrameworkCore;
using myVendingMachine.Application;
using myVendingMachine.Data;

namespace myVendingMachine.Controllers.Tests
{
    [TestClass()]
    public class VendingMachineControllerTests
    {
        Mock<IVendingService> mockVendingService = new Mock<IVendingService>();
        Mock<ITransactionService> mockTransactionService = new Mock<ITransactionService>();
        Mock<ILogger<VendingMachineController>> mockLogger = new Mock<ILogger<VendingMachineController>>();

        [TestInitialize]
        public void TestInitialize()
        {
            //add initialization code here
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //add clean up code here
        }


        [TestMethod()]
        public async Task GetItems_ReturnsOkResult_WhenProductsExist()
        {
            // Arrange
            mockVendingService.Setup(service => service.GetItems())
                .ReturnsAsync(new List<Product>
                {
                    new Product { Id = 1, Name = "Product A", Rate = 2.99M, Quantity = 10 },
                    new Product { Id = 2, Name = "Product B", Rate = 1.49M , Quantity = 20 }
                });

            var controller = new VendingMachineController(mockVendingService.Object,mockTransactionService.Object,mockLogger.Object);

            // Act
            var result = await controller.GetItems();

            // Assert
            var okResult = result.Result as OkObjectResult ; // as ActionResult<IEnumerable<Product>>;
            
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var products = okResult.Value as IEnumerable<Product>;
            Assert.IsNotNull(products);
            Assert.AreEqual(2, products.Count());
        }

        [TestMethod]
        public async Task Balance_ReturnsOkResult_WithCorrectBalance()
        {
            // Arrange
            decimal expectedBalance = 20.0m; // Expected balance value - check with DB Transaction Table
            string expectedBalanceString = $"Available Balance is {expectedBalance:C2}.";

            mockVendingService.Setup(service => service.GetCurrentBalance())
                .ReturnsAsync(expectedBalance);

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.Balance();

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            string? balance = okResult.Value as string; 
            Assert.AreEqual(expectedBalanceString, balance);
        }

        [TestMethod]
        public async Task Purchase_ReturnsOkResult_WithCorrectBalance()
        {
            // Arrange
            decimal expectedBalance = 20.0m; // Expected balance value - check with DB Transaction Table
            string expectedBalanceString = $"Available Balance is {expectedBalance:C2}.";
            
            int itemId = 1;

            var mockTransactionService = new Mock<ITransactionService>();
            mockTransactionService.Setup(service => service.PurchaseItem(itemId))
                .ReturnsAsync(expectedBalance);

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.Purchase(itemId);

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            string? balance = okResult.Value as string; 
            Assert.AreEqual(expectedBalanceString, balance);
        }

        [TestMethod]
        public async Task LoadMoney_ReturnsOkResult_WithCorrectBalance()
        {
            // Arrange
            decimal amountToLoad = 10.0m; // Amount to load
            decimal previousBalance = 25.0m;
            string expectedBalance = $"Available Balance is {amountToLoad + previousBalance:C2}.";

            var mockVendingService = new Mock<IVendingService>();
            mockVendingService.Setup(service => service.LoadMoney(amountToLoad))
                .ReturnsAsync(amountToLoad+previousBalance);

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.LoadMoney(amountToLoad);

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var currBalance = okResult.Value as string;

            Assert.IsNotNull(currBalance);
            Assert.AreEqual(expectedBalance, currBalance);
        }

        [TestMethod]
        public async Task ReturnMoney_ReturnsOkResult_WithCorrectBalance()
        {
            // Arrange
            decimal currentBalance = 25.0m;
            string expectedBalance = $"Balance Returned : {currentBalance:C2}.";

            var mockTransactionService = new Mock<ITransactionService>();
            mockTransactionService.Setup(service => service.ReturnMoney())
                .ReturnsAsync(currentBalance);

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.ReturnMoney();

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var currBalance = okResult.Value as string;

            Assert.IsNotNull(currBalance);
            Assert.AreEqual(expectedBalance, currBalance);
        }

        [TestMethod]
        public async Task Receipt_ReturnsOkResult_WithReceipt()
        {
            // Arrange
            Product product1 = new Product { Id = 1, Name = "Product A", Rate = 2.99M, Quantity =10 };
            Product product2 = new Product { Id = 2, Name = "Product B", Rate = 1.49M, Quantity = 15 };
            List<Product> productList = new List<Product>();

            productList.Add(product1);
            productList.Add(product2);

            var mockTransctionService = new Mock<ITransactionService>();
            mockTransactionService.Setup(service => service.Receipt())
                .ReturnsAsync(new Receipt
                {
                    ReceiptId = 1,
                    UserId = "MACID001",
                    PurchaseDate = new DateTime(2023, 08, 31),
                    TotalAmount = 50.00m,
                    LoadedAmount = 200.00m,
                    CurrentBalance = 150.00m,
                    Items = productList
                });

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.Receipt();

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var receipt = okResult.Value as Receipt;

            Assert.IsNotNull(receipt);
            Assert.AreEqual(1, receipt.ReceiptId);
            Assert.IsNotNull(receipt.PurchaseDate);
        }

        [TestMethod]
        public async Task AddProduct_ReturnsOkResult_WithMessage()
        {
            // Arrange
            var newProduct = new Product
            {
                Id = 1,
                Name = "Product X",
                Rate = 2.99M,
                Quantity = 20
            };

            string expectedResult = $"Product {newProduct.Name} Added Successfully.";


            var mockVendingService = new Mock<IVendingService>();
            mockVendingService.Setup(service => service.AddProduct(newProduct))
                .ReturnsAsync(newProduct);

            var controller = new VendingMachineController(mockVendingService.Object, mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = await controller.AddProduct(newProduct);

            // Assert
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var message = okResult.Value as string;

            Assert.IsNotNull(message);
            Assert.AreEqual(expectedResult, message);
        }

        //[TODO] remaing unit tests

    }
}