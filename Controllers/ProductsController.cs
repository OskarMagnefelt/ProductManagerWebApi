using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using product_manager_webapi.DTOs.ProductDtos;
using ProductManager.Data;
using ProductManager.Data.Entities;

namespace ProductManager.Controllers;

[ApiController]
// [Authorize]
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
    [HttpGet("name")]
    public ActionResult<ProductDto> GetProductsByName([FromQuery] string? name)
    {
        var productsWithName = context.Product
            .Where(p => p.Name == name)
            .ToList();

        if (productsWithName.Count == 0)
        {
            return NotFound("Inga produkter utav detta namn hittades.");
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
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> SearchProductBySKU(string sku)
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
    public ActionResult<ProductDto> AddProduct(AddProductRequestDto request)
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

        // return Created("", productDto);

        return CreatedAtAction( // 201 Created
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
    /// If no product with the specified SKU is found, it returns a 404 Not Found response.
    /// </returns>
    /// <response code="204">Product successfully updated.</response>
    /// <response code="404">If no product with the specified SKU is found.</response>
    [HttpPut("{sku}")]
    public ActionResult<ProductDto> UpdateProduct(string sku, UpdateProductRequestDto updateProductRequest)
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
        // updateProductRequest.SKU = product.SKU;
        product.Description = updateProductRequest.Description;
        product.Image = updateProductRequest.Image;
        product.Price = updateProductRequest.Price;

        context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Retrieves product information by its SKU.
    /// </summary>
    /// <remarks>
    /// This endpoint allows you to retrieve detailed information about a product by providing its SKU.
    /// </remarks>
    /// <param name="id">The SKU of the product to retrieve information for.</param>
    /// <returns>
    /// If a product with the specified SKU is found, it returns a 200 (OK) response with the product details in the body.
    /// If no product is found with the specified SKU, it returns a 404 (Not Found) response.
    /// </returns>
    /// <response code="200">Returns the product information with the specified SKU.</response>
    /// <response code="404">If no product with the specified SKU is found.</response>
    [HttpGet("api/products/{sku}")]
    public ActionResult<ProductInfoDto> GetProductInfo(string sku)
    {
        try
        {
            // Retrieve the product by its ID
            var product = context.Product
                .Where(p => p.SKU == sku)
                .Select(p => new ProductInfoDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }


    private readonly ApplicationDbContext context;

    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }
}
