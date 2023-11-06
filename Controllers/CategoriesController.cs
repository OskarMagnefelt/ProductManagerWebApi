using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using product_manager_webapi.DTOs.CategoryDtos;
using ProductManager.Data;
using ProductManager.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace ProductManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CategoriesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves a list of all categories.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to retrieve a list of all categories available in the system.
        /// </remarks>
        /// <returns>An IEnumerable of categories.</returns>
        [HttpGet]
        public IEnumerable<CategoryDto> GetCategories()
        {
            var categories = context.Category.ToList();

            IEnumerable<CategoryDto> categoriesDto = categories.Select(x => new CategoryDto
            {
                Name = x.Name,
                Id = x.Id
            });

            // return Ok(categoriesDto);
            return categoriesDto;
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the category.</param>
        /// <returns>
        /// A 200 OK response containing the category with the ID.
        /// If no category is found with the given ID, a 404 Not Found response is returned.
        /// </returns>
        /// <response code="200">Category retrieved successfully.</response>
        /// <response code="404">Category with the specified ID not found.</response>
        [HttpGet("{id}")]
        public ActionResult<CategoryDto> GetCategoryById(int id)
        {
            var category = context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="category">The category object representing the category to be created.</param>
        /// <returns>
        /// A 201 Created response containing the newly created category.
        /// If the category creation fails, a 400 Bad Request response is returned.
        /// </returns>
        /// <response code="201">Product category created successfully.</response>
        /// <response code="400">Bad request. Failed to create the product category.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDto), 201)] // Specifies the expected response type and status code 201
        [ProducesResponseType(400)] // Specifies status code 400 without a response type
        public ActionResult<CategoryDto> AddCategory(CategoryDto request)
        {
            var category = new Category
            {
                Id = request.Id,
                Name = request.Name
            };
            context.Category.Add(category);

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("Failed to create the product.");
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            // return Created("", categoryDto);

            return CreatedAtAction( // 201 Created
                nameof(GetCategoryById),
                new { id = category.Id },
                categoryDto);
        }


        [HttpPost("{id}/products")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AddProductToCategoryDTO), 201)] // Specifies the expected response type and status code 201
        [ProducesResponseType(400)] // Specifies status code 400 without a response type
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
