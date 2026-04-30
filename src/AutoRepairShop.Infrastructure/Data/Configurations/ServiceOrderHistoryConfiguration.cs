using AutoRepairShop.Domain.Entities.ServiceOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderHistoryConfiguration : IEntityTypeConfiguration<ServiceOrderHistory>
    {
        public void Configure(EntityTypeBuilder<ServiceOrderHistory> builder)
        {
            builder.ToTable("ServiceOrderHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ServiceOrderId).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired();

            builder.HasIndex(x => x.ServiceOrderId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
