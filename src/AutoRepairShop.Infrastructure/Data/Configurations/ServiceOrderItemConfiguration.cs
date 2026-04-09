using AutoRepairShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderItemConfiguration : IEntityTypeConfiguration<ServiceOrderItem>
    {
        public void Configure(EntityTypeBuilder<ServiceOrderItem> builder)
        {
            builder.ToTable("ServiceOrderItem");

            builder.HasKey(c => c.Id);

            builder.Property(x => x.SupplyId)
                   .IsRequired();

            builder.Property(x => x.ServiceOrderId)
                   .IsRequired();

            builder.HasIndex(x => x.SupplyId);
            builder.HasIndex(x => x.ServiceOrderId);
        }
    }
}
