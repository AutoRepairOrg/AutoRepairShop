using AutoRepairShop.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class CustomerEntityConfiguration : IEntityTypeConfiguration<CustomerEntity>
    {
        public void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

            builder.Property(c => c.Document).IsRequired().HasColumnName("Document");

            builder.Property(c => c.Phone).IsRequired();

            builder.Property(c => c.Email).IsRequired().HasMaxLength(150);

            builder.Property(c => c.Username).IsRequired();

            builder.Property(c => c.Password).IsRequired();
        }
    }
}
