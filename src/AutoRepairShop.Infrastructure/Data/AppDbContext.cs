using AutoRepairShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoRepairShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
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
        }
    }
}
