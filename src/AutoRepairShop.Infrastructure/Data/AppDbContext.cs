using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoRepairShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
        public DbSet<AdminEntity> Admins => Set<AdminEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
        public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
        public DbSet<ServiceOrderEntity> ServiceOrders => Set<ServiceOrderEntity>();
        public DbSet<ServiceOrderHistoryEntity> ServiceOrderHistories => Set<ServiceOrderHistoryEntity>();
        public DbSet<SupplyEntity> Supplies => Set<SupplyEntity>();
        public DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            SeedAdmins(modelBuilder);
            SeedCustomers(modelBuilder);
        }

        private void SeedCustomers(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CustomerEntity>()
                .HasData(
                    new CustomerEntity
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Cliente Demo",
                        Document = "52998224725",
                        Phone = "51999999999",
                        Username = "customer",
                        Password = PasswordSeed.Hash,
                    }
                );
        }

        private void SeedAdmins(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AdminEntity>()
                .HasData(
                    new AdminEntity
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "Admin Master",
                        Department = "Sistema",
                        Username = "admin",
                        Password = PasswordSeed.Hash,
                    }
                );
        }
    }

    public static class PasswordSeed
    {
        public const string Hash = "$2a$11$7/f7K3hTZxwP1CxlMnMW0uwiPawUUB1ga5c7KFNycp/w3NX.DrZOO";
    }
}
