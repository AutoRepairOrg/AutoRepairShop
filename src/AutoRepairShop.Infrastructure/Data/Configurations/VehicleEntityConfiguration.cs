using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class VehicleEntityConfiguration : IEntityTypeConfiguration<VehicleEntity>
    {
        public void Configure(EntityTypeBuilder<VehicleEntity> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();

            builder.Property(x => x.Plate).IsRequired().HasMaxLength(10);

            builder.Property(x => x.Brand).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Model).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Year).IsRequired();

            builder.HasIndex(x => x.CustomerId);
        }
    }
}
