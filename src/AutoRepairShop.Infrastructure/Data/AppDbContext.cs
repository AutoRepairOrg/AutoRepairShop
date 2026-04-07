using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoRepairShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var documentConverter = new ValueConverter<Document, string>(
                document => document.Value,         
                value => new Document(value));

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

                entity.Property(c => c.Document)
                .IsRequired()
                .HasMaxLength(20);

                entity.Property(c => c.Document)
                .HasConversion(documentConverter)
                .HasColumnName("Document")
                .IsRequired();
            });
        }
    }
}
