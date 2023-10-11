using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using product_manager_webapi.DTOs.CategoryDtos;
using ProductManager.Data;
using ProductManager.Data.Entities;
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
        /// Retrieves a list of all categories.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to retrieve a list of all categories available in the system.
        /// </remarks>
        /// <returns>An IEnumerable of categories.</returns>
        [HttpGet]
        public IEnumerable<CategoryNameDto> GetCategories()
        {
            var categories = context.Category.ToList();

            IEnumerable<CategoryNameDto> categoriesNamesDto = categories.Select(x => new CategoryNameDto
            {
                Name = x.Name,
            });

            // return Ok(categoriesDto);
            return categoriesNamesDto;
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
        public ActionResult<CategoryDto> AddCategory(AddCategoryRequestDto request)
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
