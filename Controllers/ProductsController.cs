using Microsoft.AspNetCore.Mvc;
using ProductManager.Data;
using ProductManager.Data.Entities;

namespace ProductManager.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all products.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to retrieve a list of all products available in the system.
    /// </remarks>
    /// <returns>An IEnumerable of products.</returns>
    [HttpGet]
    public IEnumerable<Product> GetProducts()
    {
        var products = context.Product.ToList();

        return products;
    }

    /// <summary>
    /// Retrieves products with a specific name.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to search for products with a given name.
    /// </remarks>
    /// <param name="name">The name to search for.</param>
    /// <returns>
    /// If products with the specified name are found, it returns an OK response with the list of matching products.
    /// If no products are found, it returns a NotFound response.
    /// </returns>
    /// <response code="200">Returns a list of products with the specified name.</response>
    /// <response code="404">If no products are found with the specified name.</response>
    [HttpGet("search")]
    public IActionResult GetProductsByName([FromQuery] string name)
    {
        var productsWithName = context.Product
            .Where(p => p.Name == name)
            .ToList();

        if (productsWithName.Count == 0)
        {
            return NotFound("Inga produkter utav detta namn hittades.");
        }

        return Ok(productsWithName);
    }

    /// <summary>
    /// Searches for a product by its SKU property.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to search for a product by providing its SKU (Stock Keeping Unit) property.
    /// If a product with the specified SKU is found, it will be returned in the response.
    /// </remarks>
    /// <param name="sku">The SKU (Stock Keeping Unit) of the product to search for.</param>
    /// <returns>
    /// If a product with the specified SKU is found, it returns a 200 (OK) response with the product details in the body.
    /// If no product is found with the specified SKU, it returns a 404 (Not Found) response.
    /// </returns>
    /// <response code="200">Returns the product with the specified SKU.</response>
    /// <response code="404">If no product is found with the specified SKU.</response>

    //Är det okej att döpa endpointen till vad man tycker är bäst lämpat?
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SearchProductBySKU(string sku)
    {
        var product = context.Product.SingleOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to create a new product by providing the necessary product details.
    /// </remarks>
    /// <param name="product">The product object to create.</param>
    /// <returns>
    /// If the product is successfully created, it returns a 201 Created response with the created product.
    /// If the creation fails, it returns a 400 Bad Request response with an error message.
    /// </returns>
    /// <response code="201">Returns the newly created product.</response>
    /// <response code="400">If the creation of the product fails.</response>
    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        context.Product.Add(product);

        try
        {
            context.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest("Failed to create the product.");
        }

        return Created("", product);
    }

    /// <summary>
    /// Deletes a product by SKU.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to delete a product by providing its SKU (Stock Keeping Unit).
    /// </remarks>
    /// <param name="sku">The SKU of the product to delete.</param>
    /// <returns>
    /// If the product is successfully deleted, it returns a 204 No Content response.
    /// If no product with the specified SKU is found, it returns a 404 Not Found response.
    /// </returns>
    /// <response code="204">Product successfully deleted.</response>
    /// <response code="404">If no product with the specified SKU is found.</response>
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

    /// <summary>
    /// Updates a product by SKU.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to update a product by providing its SKU (Stock Keeping Unit)
    /// and a new set of product information.
    /// </remarks>
    /// <param name="sku">The SKU of the product to update.</param>
    /// <param name="updatedProduct">The updated product information.</param>
    /// <returns>
    /// If the product is successfully updated, it returns a 200 OK response with the updated product.
    /// If no product with the specified SKU is found, it returns a 404 Not Found response.
    /// </returns>
    /// <response code="200">Product successfully updated.</response>
    /// <response code="404">If no product with the specified SKU is found.</response>
    [HttpPut("{sku}")]
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
