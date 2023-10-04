using System.ComponentModel.DataAnnotations;

namespace ProductManager.Domain;
public class Category
{
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    public ICollection<ProductCategory>
    ProductCategories
    { get; set; } = new List<ProductCategory>();
}