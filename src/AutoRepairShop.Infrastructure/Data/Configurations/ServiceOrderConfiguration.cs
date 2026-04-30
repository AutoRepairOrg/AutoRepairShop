using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
    {
        public void Configure(EntityTypeBuilder<ServiceOrder> builder)
        {
            builder.ToTable("ServiceOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();

            builder.Property(x => x.VehicleId).IsRequired();

            builder.Property(x => x.Status).HasConversion<string>().IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.StartedAt);

            builder.Property(x => x.FinishedAt);

            builder.Navigation(x => x.Services).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(x => x.Supplies).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(x => x.History).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne<Vehicle>()
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(
                x => x.Services,
                services =>
                {
                    services.ToTable("ServiceOrderServices");

                    services.WithOwner().HasForeignKey(x => x.ServiceOrderId);

                    services.HasKey(x => new { x.ServiceOrderId, x.ServiceId });

                    services.Property(x => x.ServiceOrderId).IsRequired();
                    services.Property(x => x.ServiceId).IsRequired();

                    services
                        .HasOne<Service>()
                        .WithMany()
                        .HasForeignKey(x => x.ServiceId)
                        .OnDelete(DeleteBehavior.Restrict);
                }
            );

            builder.OwnsMany(
                x => x.Supplies,
                supplies =>
                {
                    supplies.ToTable("ServiceOrderSupplies");

                    supplies.WithOwner().HasForeignKey(x => x.ServiceOrderId);

                    supplies.HasKey(x => new { x.ServiceOrderId, x.SupplyId });

                    supplies.Property(x => x.ServiceOrderId).IsRequired();
                    supplies.Property(x => x.SupplyId).IsRequired();
                    supplies.Property(x => x.Quantity).IsRequired();

                    supplies
                        .HasOne<Supply>()
                        .WithMany()
                        .HasForeignKey(x => x.SupplyId)
                        .OnDelete(DeleteBehavior.Restrict);
                }
            );

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
