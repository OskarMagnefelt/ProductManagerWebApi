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

        [HttpGet]
        public IActionResult GetProductCategories()
        {
            try
            {
                var categoriesWithProducts = context.Category
                    .Include(c => c.Products) // Include the related products
                    .Select(c => new ProductCategoriesDto
                    {
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        Products = c.Products.Select(p => new ProductDto
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



        [HttpPost("add")]
        public IActionResult AddProductToCategory([FromBody] AddProductToCategoryRequestDTO request)
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

            return Created("", request);
        }

    }
}
