using AutoRepairShop.Domain.Entities;
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

            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.Property(x => x.VehicleId)
                   .IsRequired();

            builder.Property(x => x.ServiceId)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(x => x.TotalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.StartedAt);

            builder.Property(x => x.FinishedAt);

            builder.Navigation(x => x.Items)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<Service>()
                   .WithMany()
                   .HasForeignKey(x => x.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Vehicle>()
                   .WithMany()
                   .HasForeignKey(x => x.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Items, items =>
            {
                items.ToTable("ServiceOrderItems");

                items.WithOwner()
                     .HasForeignKey("ServiceOrderId");

                // Shadow key (required by EF Core)
                items.Property<Guid>("Id");
                items.HasKey("Id");

                items.Property(i => i.SupplyId)
                     .IsRequired();

                items.Property(i => i.SupplyName)
                     .IsRequired()
                     .HasMaxLength(200);

                items.Property(i => i.UnitPrice)
                     .HasPrecision(18, 2)
                     .IsRequired();

                items.Property(i => i.Quantity)
                     .IsRequired();
            });

            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.ServiceId);
        }
    }
}