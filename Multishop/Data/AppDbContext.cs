using Microsoft.EntityFrameworkCore;
using Multishop.Models;
using Multishop.Models.Base;
using System.Reflection;

namespace Multishop.Data;

public class AppDbContext:DbContext
{
	public AppDbContext(DbContextOptions options):base(options)
	{

	}

	public DbSet<Slide> Slides { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<ProductImage> ProductImages { get; set; }
	public DbSet<Setting> Settings { get; set; }
	public DbSet<Size> Sizes { get; set; }
	public DbSet<Color> Colors { get; set; }
	public DbSet<ProductSize> ProductSizes { get; set; }
	public DbSet<ProductColor> ProductColors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entites = ChangeTracker.Entries<BaseEntity>();
        foreach (var data in entites)
        {
            switch (data.State)
            {
                case EntityState.Modified:
                    data.Entity.UpdatedTime = DateTime.UtcNow;
                    break;
                case EntityState.Added:
                    data.Entity.CreatedTime = DateTime.UtcNow;
                    break;
                default:
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
