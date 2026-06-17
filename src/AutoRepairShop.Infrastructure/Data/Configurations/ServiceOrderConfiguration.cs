using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrderEntity>
    {
        public void Configure(EntityTypeBuilder<ServiceOrderEntity> builder)
        {
            builder.ToTable("ServiceOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();

            builder.Property(x => x.VehicleId).IsRequired();

            builder.Property(x => x.Status).HasConversion<string>().IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.StartedAt);

            builder.Property(x => x.FinishedAt);

            builder
                .HasOne<CustomerEntity>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne<VehicleEntity>()
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(x => x.Services)
                .WithOne()
                .HasForeignKey(x => x.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Supplies)
                .WithOne()
                .HasForeignKey(x => x.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.History)
                .WithOne()
                .HasForeignKey(x => x.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.Status);
        }
    }
}
