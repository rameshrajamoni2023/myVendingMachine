using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using myVendingMachine.Models;
using System.Reflection;

namespace myVendingMachine.Data
{
        public class TransactionDetailsDbContext : DbContext
        {
            public TransactionDetailsDbContext(DbContextOptions<TransactionDetailsDbContext> options)
                : base(options)
            {

            }

            public DbSet<Product> Product { get; set; }
            public DbSet<Transactions> Transactions { get; set; }
            public DbSet<TransactionDetails> TransactionDetails { get; set; }
            

        }
}
