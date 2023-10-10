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

    [HttpGet("name={name}")]
    public IActionResult GetProductsByName(string name)
    {
        // Retrieve products with the given name
        var productsWithName = context.Product
            .Where(p => p.Name == name)
            .ToList();

        if (productsWithName.Count == 0)
        {
            return NotFound("Inga produkter utav detta namn hittades.");
        }

        return Ok(productsWithName);
    }

    [HttpGet("{sku}")]
    public IActionResult SearchProductBySKU(string sku)
    {
        var product = context.Product.SingleOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost("Add Product")]
    public IActionResult AddProduct(Product product)
    {
        if (product == null)
        {
            return BadRequest("Invalid product data. Product cannot be null.");
        }

        context.Product.Add(product);

        try
        {
            context.SaveChanges();
        }
        catch (Exception)
        {
            // If an exception occurs during save, return a 400 Bad Request response
            return BadRequest("Failed to create the product.");
        }

        return Created("", product);
    }

    [HttpDelete("Delete")]
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

    [HttpPut("Update")]
    public IActionResult UpdateProduct(string sku, Product updatedProduct)
    {
        var product = context.Product.SingleOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Image = updatedProduct.Image;
        product.Price = updatedProduct.Price;

        context.SaveChanges();

        return Ok(product);
    }

    private readonly ApplicationDbContext context;

    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }
}
