using System.ComponentModel.DataAnnotations;

namespace ProductManager.Data.Entities;
public class Category
{
    public int Id { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}