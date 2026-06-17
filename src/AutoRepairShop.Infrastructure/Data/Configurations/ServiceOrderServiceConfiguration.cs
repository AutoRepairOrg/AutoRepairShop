using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceOrderServiceConfiguration
        : IEntityTypeConfiguration<ServiceOrderServiceEntity>
    {
        public void Configure(EntityTypeBuilder<ServiceOrderServiceEntity> builder)
        {
            builder.ToTable("ServiceOrderServices");

            builder.HasKey(x => new { x.ServiceOrderId, x.ServiceId });

            builder.Property(x => x.ServiceOrderId).IsRequired();
            builder.Property(x => x.ServiceId).IsRequired();

            builder
                .HasOne<ServiceEntity>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
