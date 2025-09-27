using FalveyInsuranceGroup.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FalveyInsuranceGroup.Db
{
    // Allows us to interact with our database
    public class FalveyInsuranceGroupContext : DbContext
    {
        public FalveyInsuranceGroupContext(DbContextOptions<FalveyInsuranceGroupContext> options):base(options) 
        { 
        }
        
        // Override method from the base DbContext class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasData();
            modelBuilder.Entity<Employee>().HasData();
            modelBuilder.Entity<Policy>().HasData();
            //modelBuilder.Entity<CustomerRecord>().HasData();
            modelBuilder.Entity<CustomerRecord>().ToTable("customer_records");
        }

        // Specify DbSet instance for operating with databases
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }       
        public DbSet<Policy> Policies { get; set; }
        public DbSet<CustomerRecord> CustomerRecords { get; set; }
    }
}
