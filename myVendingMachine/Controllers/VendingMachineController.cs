using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using myVendingMachine.Application.Contracts;
using myVendingMachine.Data;
using myVendingMachine.Helper;
using myVendingMachine.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace myVendingMachine.Controllers
{

    [Route("api/vendingmachine")]
    [ApiController]
    public class VendingMachineController : ControllerBase
    {
        //private readonly VendingMachineDbContext _context;
        //private readonly TransactionDetailsDbContext _transactionDetailsDbContext;
        private readonly ILogger<VendingMachineController> _logger;
        
        private readonly IVendingService _vendingService;
        private readonly ITransactionService _transactionService;

        public VendingMachineController(IVendingService vendingService, ITransactionService transactionService, ILogger<VendingMachineController> logger)
        {
            _vendingService = vendingService;
            _transactionService = transactionService;
            _logger = logger;

            _logger.LogInformation("Application Started.");
        }

        // GET: api/vendingmachine/items
        [HttpGet("reset")]
        public async Task<ActionResult<string>> Reset()
        {
            _logger.LogInformation("**********Reset Called**********");

            try 
            {    
                string result = await _transactionService.ResetDetails();

                _logger.LogInformation(result);
                return Ok(result);
            }
            catch(VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/vendingmachine/items
        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<Product>>> GetItems()
        {
            _logger.LogInformation("**********GetItems Called**********");

            try
            {
                var items = await _vendingService.GetItems();
                return Ok(items);
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/vendingmachine/items
        [HttpGet("balance")]
        public async Task<ActionResult<string>> Balance()
        {
            _logger.LogInformation("**********Balance Called**********");

            try
            { 
                decimal balance = await _vendingService.GetCurrentBalance();
                return Ok($"Available Balance is {balance:C2}.");
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/vendingmachine/purchase/{itemId}
        [HttpPost("purchase/{itemId}")]
        public async Task<ActionResult<decimal>> Purchase(int itemId)
        {
            _logger.LogInformation("**********Purchase Called**********");

            try
            { 
                decimal balance = await _transactionService.PurchaseItem(itemId);

                // Implement payment - gateway,validations etc
                //[TODO] 

                // item dispensing logic here - update inventory etc.
                //[TODO]
                
                return Ok($"Available Balance is {balance:C2}.");

            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            
        }

        // POST: api/vendingmachine/addfunds
        [HttpPost("loadMoney")]
        public async Task<ActionResult<string>> LoadMoney([FromBody] decimal amount)
        {
            _logger.LogInformation("**********LoadMoney Called**********");

            try
            {
                decimal balance = await _vendingService.LoadMoney(amount);

                return Ok($"Available Balance is {balance:C2}.");
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        // GET: api/vendingmachine/addfunds
        [HttpGet("ReturnMoney")]
        public async Task<ActionResult<string>> ReturnMoney()
        {
            _logger.LogInformation("**********ReturnMoney Called**********");

            try
            {
                decimal balance = await _transactionService.ReturnMoney();
                                
                return Ok($"Balance Returned : {balance:C2}.");

            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }


        }

        [HttpGet("Receipt")]
        public async Task<ActionResult<Receipt>> Receipt()
        {
            _logger.LogInformation("**********Receipt Called**********");
            
            try
            {
               var receipt = await _transactionService.Receipt();
               return Ok(receipt);
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult<string>> AddProduct(Product product)
        {
            _logger.LogInformation("**********AddProduct Called**********");
            
            try
            {
                Product prod = await _vendingService.AddProduct(product);

                return Ok($"Product {prod.Name} Added Successfully.");
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost("AddProductQuantity")]
        public async Task<ActionResult<string>> AddProductQuantity(int productId,int quantity)
        {
            _logger.LogInformation("**********AddProductQuantity Called**********");

            try
            {
                int updatedQuantity = await _vendingService.UpdateProductQuantity(productId,quantity);

                return Ok($"Quantity After Refill is {updatedQuantity}");
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        
        //nice to have to bulk upload products
        [HttpPost("UploadProducts")]
        public ActionResult<string> UploadProducts([FromForm] IFormFile file, IVendingService _vendingService)
        {
            _logger.LogInformation("**********UploadProducts Called**********");

            try
            {
                if (file == null || file.Length <= 0)
                {
                    _logger.LogError("Invalid File");
                    return BadRequest("Invalid file.");
                }

                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(new System.Globalization.CultureInfo("en-US"))))
                {
                    var records =  csv.GetRecords<Product>().ToList();

                    foreach(Product row in records)
                    {
                        Task<Product> prod = _vendingService.AddProduct(row);        //temporary need to fix this
                    }

                    _logger.LogInformation("{0} Products uploaded successfully.", records.Count);
                    return Ok(records);
                }
            }
            catch (VendingMachineException vex)
            {
                _logger.LogWarning(vex.Message);
                return Ok(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }


}
