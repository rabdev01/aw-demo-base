using Microsoft.EntityFrameworkCore;
using AdventureWorksWeb.Models;

namespace AdventureWorksWeb.Data;

public class AdventureWorksContext : DbContext
{
    public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product", "SalesLT");
            entity.HasKey(e => e.ProductID);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.ProductCategoryID);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategory", "SalesLT");
            entity.HasKey(e => e.ProductCategoryID);
            entity.HasOne(e => e.ParentCategory)
                  .WithMany()
                  .HasForeignKey(e => e.ParentProductCategoryID);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer", "SalesLT");
            entity.HasKey(e => e.CustomerID);
        });
    }
}
