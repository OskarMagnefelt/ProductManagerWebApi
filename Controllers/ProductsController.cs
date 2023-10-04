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

    [HttpPut("{sku}")]
    public IActionResult UpdateProduct(string sku, Product updatedProduct)
    {
        var product = context.Product.SingleOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        // Update the properties of the existing product with the new values
        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Image = updatedProduct.Image;
        product.Price = updatedProduct.Price;

        context.SaveChanges();

        return Ok(product);
    }

    [HttpGet("search")]
        public IActionResult SearchProductBySKU(string sku)
        {
            var product = context.Product.SingleOrDefault(p => p.SKU == sku);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

    private readonly ApplicationDbContext context;

    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }
}
