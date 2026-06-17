using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderSupplyConfiguration
        : IEntityTypeConfiguration<ServiceOrderSupplyEntity>
    {
        public void Configure(EntityTypeBuilder<ServiceOrderSupplyEntity> builder)
        {
            builder.ToTable("ServiceOrderSupplies");

            builder.HasKey(x => new { x.ServiceOrderId, x.SupplyId });

            builder.Property(x => x.ServiceOrderId).IsRequired();
            builder.Property(x => x.SupplyId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            builder
                .HasOne<SupplyEntity>()
                .WithMany()
                .HasForeignKey(x => x.SupplyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
