using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class SupplyConfiguration : IEntityTypeConfiguration<SupplyEntity>
    {
        public void Configure(EntityTypeBuilder<SupplyEntity> builder)
        {
            builder.ToTable("Supplies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

            builder.Property(c => c.StockQuantity).IsRequired();

            builder.Property(c => c.Price).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
