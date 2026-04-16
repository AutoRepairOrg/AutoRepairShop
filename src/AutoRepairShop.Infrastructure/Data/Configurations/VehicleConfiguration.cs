using AutoRepairShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.CustomerId);

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Plate, plate =>
            {
                plate.Property(p => p.Value)
                     .HasColumnName("Plate")
                     .HasMaxLength(10)
                     .IsRequired();
            });

            builder.Property(x => x.Brand)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Model)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Year)
                   .IsRequired();

            builder.HasIndex(x => x.CustomerId);
        }
    }
}
