using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Multishop.Models;

namespace Multishop.Configurations
{
    public class SlideConfiguration : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.Property(s => s.Title).IsRequired().HasMaxLength(50);
            builder.HasIndex(s => s.Title).IsUnique();

            builder.Property(s => s.Description).IsRequired(false).HasMaxLength(100);
            builder.Property(s => s.ImageUrl).IsRequired();

            builder.HasIndex(s => s.Order).IsUnique();
        }
    }
}
