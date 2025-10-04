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

            modelBuilder.Entity<Customer>();
            modelBuilder.Entity<CustomerRecord>().ToTable("customer_records");
            modelBuilder.Entity<Employee>()
                .HasIndex(employee_index => employee_index.email)
                .IsUnique();
            modelBuilder.Entity<Policy>();
            modelBuilder.Entity<User>()
                .HasIndex(user_index => user_index.email)
                .IsUnique();

        }

        // Specify DbSet instance for operating with databases
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerRecord> CustomerRecords { get; set; }
        public DbSet<Employee> Employees { get; set; }       
        public DbSet<Policy> Policies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Release> Releases { get; set; }

    }
}
