using AutoRepairShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
        public DbSet<ServiceOrderItem> ServiceOrderItems => Set<ServiceOrderItem>();
        public DbSet<Supply> Supplies => Set<Supply>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();


        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                 typeof(AppDbContext).Assembly
            );

            SeedAdmins(modelBuilder);
            SeedCustomers(modelBuilder);
        }

        private void SeedCustomers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Cliente Demo",
                    Document =  new Document("52998224725"),
                    Phone = "51999999999",
                    Username = "customer",
                    Password = PasswordSeed.Hash
                }
            );
        }

        private void SeedAdmins(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Admin Master",
                    Department = "Sistema",
                    Username = "admin",
                    Password = PasswordSeed.Hash
                }
            );
        }
    }

    public static class PasswordSeed
    {
        public static string Hash = BCrypt.Net.BCrypt.HashPassword("123456");
    }
} 
