using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Domain;
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

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = context.Category.ToList();
            return Ok(categories);
        }

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

        [HttpPost]
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
