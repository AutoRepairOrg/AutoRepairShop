using System.Reflection.Emit;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoRepairShop.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            var documentConverter = new ValueConverter<Document, string>(
                document => document.Value,
                value => Document.Create(value)
            );

            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

            builder
                .Property(c => c.Document)
                .HasConversion(documentConverter)
                .HasColumnName("Document")
                .IsRequired();
        }
    }
}
