using Microsoft.EntityFrameworkCore;

namespace ProductManager.Data;
using ProductManager.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
    { }


    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
}
