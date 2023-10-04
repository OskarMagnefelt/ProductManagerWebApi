using Microsoft.EntityFrameworkCore;

namespace ProductManager.Data;
using ProductManager.Domain;

public class ApplicationDbContext : DbContext
{

    // Detta ska ersättas med Dependency Injections

    // private string connectionString = "Server=.;Database=FreakyFashion;Integrated Security=true;Encrypt=False";

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlServer(connectionString);
    // }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
    { }


    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>()
            .HasKey(pc => new { pc.ProductId, pc.CategoryId });

        modelBuilder.Entity<ProductCategory>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.ProductId);

        modelBuilder.Entity<ProductCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId);
    }
}
