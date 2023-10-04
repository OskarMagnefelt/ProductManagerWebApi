using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Domain;
using System.Linq;
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
            var categories = context.ProductCategory.ToList();
            return Ok(categories);
        }

        public class AddProductToCategoryRequest
        {
            public int CategoryId { get; set; }
            public int ProductId { get; set; }
        }


        [HttpPost("add")]
        public IActionResult AddProductToCategory([FromBody] AddProductToCategoryRequest request)
        {
            // Find the category by its ID
            var category = context.Category.Find(request.CategoryId);

            if (category == null)
            {
                // Add debugging output
                Console.WriteLine($"Category with ID {request.CategoryId} not found.");
                return NotFound("Category not found.");
            }

            // Find the product by its ID
            var product = context.Product.Find(request.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Check if the product is already associated with the category
            var existingAssociation = context.ProductCategory
                .SingleOrDefault(pc => pc.CategoryId == request.CategoryId && pc.ProductId == request.ProductId);

            if (existingAssociation != null)
            {
                return Conflict("Product is already associated with the category.");
            }

            // Create a new ProductCategory entity to represent the association
            var productCategory = new ProductCategory
            {
                CategoryId = request.CategoryId,
                ProductId = request.ProductId
            };

            context.ProductCategory.Add(productCategory);
            context.SaveChanges();

            return Created("", productCategory);
        }

    }
}
