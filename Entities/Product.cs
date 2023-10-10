using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.Entities;

[Index(nameof(SKU), IsUnique = true)]
public class Product
{
    public int Id { get; set; }

    [MaxLength(50)]
    // [Required]
    public string Name { get; set; }

    [MaxLength(20)]
    [Required]
    public string SKU { get; set; }

    [MaxLength(50)]
    [Required]
    public string Description { get; set; }

    [MaxLength(50)]
    [Required]
    public string Image { get; set; }

    [Required]
    public decimal Price { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
