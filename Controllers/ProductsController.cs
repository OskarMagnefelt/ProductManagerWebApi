using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using product_manager_webapi.DTOs.ProductDtos;
using ProductManager.Data;
using ProductManager.Data.Entities;

namespace ProductManager.Controllers;

[ApiController]
[Authorize]
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
    public IEnumerable<ProductDto> GetProducts()
    {
        var products = context.Product.ToList();

        IEnumerable<ProductDto> productsDto = products.Select(x => new ProductDto
        {
            Name = x.Name,
            SKU = x.SKU,
            Description = x.Description,
            Image = x.Image,
            Price = x.Price
        });

        return productsDto;
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
    [HttpGet("products")] // Use "products" as the route template
    public ActionResult<IEnumerable<ProductDto>> GetProductsByName([FromQuery] string name)
    {
        var productsWithName = context.Product
            .Where(p => p.Name == name)
            .ToList();

        if (productsWithName.Count == 0)
        {
            return NotFound("No products with this name were found.");
        }

        IEnumerable<ProductDto> productsWithNameDto = productsWithName.Select(x => new ProductDto
        {
            Name = x.Name,
            SKU = x.SKU,
            Description = x.Description,
            Image = x.Image,
            Price = x.Price
        });

        return Ok(productsWithNameDto);
    }


    /// <summary>
    /// Gets a product by its SKU property.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to get a product by providing its SKU (Stock Keeping Unit) property.
    /// If a product with the specified SKU is found, it will be returned in the response.
    /// </remarks>
    /// <param name="sku">The SKU (Stock Keeping Unit) of the product to search for.</param>
    /// <returns>
    /// If a product with the specified SKU is found, it returns a 200 (OK) response with the product details in the body.
    /// If no product is found with the specified SKU, it returns a 404 (Not Found) response.
    /// </returns>
    /// <response code="200">Returns the product with the specified SKU.</response>
    /// <response code="404">If no product is found with the specified SKU.</response>
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> GetProductBySKU(string sku)
    {
        var product = context.Product.FirstOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound();
        }

        var productDto = new ProductDto
        {
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };

        // return Ok(productDto);
        return productDto;
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public ActionResult<ProductDto> AddProduct(ProductDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Image = request.Image,
            Price = request.Price
        };

        context.Product.Add(product);

        try
        {
            context.SaveChanges();
        }
        catch (Exception)
        {
            return BadRequest("Failed to create the product.");
        }

        var productDto = new ProductDto
        {
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };

        return CreatedAtAction(
            nameof(GetProductsByName),
            new { name = product.Name },
            productDto);
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
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
    /// <param name="updateProductRequest">The updated product information.</param>
    /// <returns>
    /// If the product is successfully updated, it returns a 204 No Content response with the updated product.
    /// If no product with the specified SKU is found, it returns a 400 Bad Request response.
    /// If product does not exist, it returns a 404 Not Found response.
    /// </returns>
    /// <response code="400">If no product with the specified SKU is found.</response>
    /// <response code="404">If no product is found.</response>
    /// <response code="204">Product successfully updated.</response>
    [HttpPut("{sku}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), 204)] // Specifies the expected response type and status code 201
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult<ProductDto> UpdateProduct(string sku, ProductDto updateProductRequest)
    {
        if (sku != updateProductRequest.SKU)
        {
            return BadRequest("SKU does not match"); // 400
        }

        var product = context.Product.FirstOrDefault(p => p.SKU == sku);

        if (product == null)
        {
            return NotFound(); // 404
        }

        product.Name = updateProductRequest.Name;
        product.Description = updateProductRequest.Description;
        product.Image = updateProductRequest.Image;
        product.Price = updateProductRequest.Price;

        context.SaveChanges();

        return NoContent(); // 204
    }

    private readonly ApplicationDbContext context;

    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }
}
