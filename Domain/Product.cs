using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.Domain;

[Index(nameof(SKU), IsUnique = true)]
public class Product
{
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(20)]
    public required string SKU { get; set; }

    [MaxLength(50)]
    public required string Description { get; set; }

    [MaxLength(50)]
    public required string Image { get; set; }

    [MaxLength(50)]
    public required string Price { get; set; }

    public ICollection<ProductCategory>
    ProductCategories
    { get; set; } = new List<ProductCategory>();
}