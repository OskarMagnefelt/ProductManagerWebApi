using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace product_manager_webapi.Data.Entities;


[Index(nameof(Name), IsUnique = true)]
public class Role
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    // En roll kan ha flera användare och en användare kan ha flera roller
    public ICollection<User> Users { get; set; } = new List<User>();
}
