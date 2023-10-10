using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProductManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CategoriesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves a list of all product categories.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to retrieve a list of all available product categories.
        /// </remarks>
        /// <returns>
        /// A 200 OK response containing a list of product categories.
        /// If there are no categories found, an empty list is returned.
        /// </returns>
        /// <response code="200">List of product categories retrieved successfully.</response>
        /// <response code="204">No product categories found (empty response).</response>
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = context.Category.ToList();
            return Ok(categories);
        }

        /// <summary>
        /// Retrieves a product category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product category.</param>
        /// <returns>
        /// A 200 OK response containing the product category with the specified identifier.
        /// If no category is found with the given identifier, a 404 Not Found response is returned.
        /// </returns>
        /// <response code="200">Product category retrieved successfully.</response>
        /// <response code="404">Product category with the specified ID not found.</response>
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
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
        [HttpPost("/{id}/products")]
        public IActionResult AddCategory(Category category)
        {
            context.Category.Add(category);
            context.SaveChanges();
            return Created("", category);
        }


        // Dessa grejer kanske jag implementerar för frontend senare

        // [HttpPut("{id}")]
        // public IActionResult UpdateCategory(int id, Category updatedCategory)
        // {
        //     var category = context.Category.Find(id);

        //     if (category == null)
        //     {
        //         return NotFound();
        //     }

        //     // Update the properties of the existing category with the new values
        //     category.Name = updatedCategory.Name;

        //     context.SaveChanges();

        //     return Ok(category);
        // }

        // [HttpDelete("{id}")]
        // public IActionResult DeleteCategory(int id)
        // {
        //     var category = context.Category.Find(id);

        //     if (category == null)
        //     {
        //         return NotFound();
        //     }

        //     context.Category.Remove(category);
        //     context.SaveChanges();

        //     return NoContent();
        // }
    }
}
