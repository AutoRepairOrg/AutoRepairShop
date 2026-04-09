using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(c => c.Price)
            .IsRequired();
        }
    }
}
