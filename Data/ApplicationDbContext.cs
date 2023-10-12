using Microsoft.EntityFrameworkCore;

namespace ProductManager.Data;

using product_manager_webapi.Data.Entities;
using ProductManager.Data.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
    { }


    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<User> Users { get; set; }
}
