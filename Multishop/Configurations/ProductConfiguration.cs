using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Multishop.Models;

namespace Multishop.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p=>p.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(p => p.Name).IsUnique();
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(6,2)");
            builder.Property(p => p.Description).IsRequired(false).HasColumnType("text");
        }
    }
}
