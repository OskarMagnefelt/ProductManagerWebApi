using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using static System.Console;

namespace ProductManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves a list of product categories along with associated products.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of product categories, each containing information about the category
        /// and a list of associated products in that category.
        /// </remarks>
        /// <returns>
        /// A 200 OK response containing a list of product categories and their associated products.
        /// If an error occurs, a 500 Internal Server Error response is returned.
        /// </returns>
        /// <response code="200">List of product categories and associated products retrieved successfully.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet]
        public ActionResult<IEnumerable<GetProductCategoriesDto>> GetProductCategories()
        {
            try
            {
                var categoriesWithProducts = context.Category
                    .Include(c => c.Products) // Include the related products
                    .Select(c => new GetProductCategoriesDto
                    {
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        Products = c.Products.Select(p => new GetProductInCategoryDto
                        {
                            ProductId = p.Id,
                            ProductName = p.Name
                        }).ToList()
                    })
                    .ToList();

                return Ok(categoriesWithProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }



        /// <summary>
        /// Associates a product with a category.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to associate a product with a specific category.
        /// </remarks>
        /// <param name="request">The request containing the CategoryId and ProductId to associate.</param>
        /// <returns>
        /// A 201 Created response indicating that the product has been successfully associated with the category.
        /// If the category or product is not found, a 404 Not Found response is returned.
        /// If the product is already associated with the category, a 409 Conflict response is returned.
        /// </returns>
        /// <response code="201">Product associated with the category successfully.</response>
        /// <response code="400">If the request is invalid or malformed.</response>
        /// <response code="404">If the category or product is not found.</response>
        /// <response code="409">If the product is already associated with the category.</response>
        [HttpPost("add")]
        public IActionResult AddProductToCategory([FromBody] AddProductToCategoryDTO request)
        {
            var category = context.Category.Find(request.CategoryId);

            if (category == null)
            {
                Console.WriteLine($"Category with ID {request.CategoryId} not found.");
                return NotFound("Category not found.");
            }

            var product = context.Product.Find(request.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            try
            {
                context.Product.Attach(product);

                product.Categories.Add(category);

                context.SaveChanges();
            }
            catch (Exception)
            {
                WriteLine("Produkt redan tillagd");
                return Conflict("Produkt redan tillagd");
            }

            return Created("", null);
        }

    }
}
