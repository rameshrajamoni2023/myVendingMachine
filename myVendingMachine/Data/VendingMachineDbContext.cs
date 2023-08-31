using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using myVendingMachine.Models;
using System.Reflection;

namespace myVendingMachine.Data
{
    public class VendingMachineDbContext : DbContext
    {
        public VendingMachineDbContext(DbContextOptions<VendingMachineDbContext> options)
            : base(options)
        {

        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

    }
}