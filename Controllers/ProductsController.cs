using Microsoft.AspNetCore.Mvc;
using ProductManager.Data;
using ProductManager.Domain;

namespace ProductManager.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Product> GetProducts()
    {
        var products = context.Product.ToList();

        return products;
    }

    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        context.Product.Add(product);

        context.SaveChanges();

        return Created("", product);
    }

    [HttpDelete("{sku}")]
    public IActionResult DeleteProduct(string sku)
    {
        var product = context.Product.SingleOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        context.Product.Remove(product);
        context.SaveChanges();

        return NoContent();
    }

    private readonly ApplicationDbContext context;

    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }
}
