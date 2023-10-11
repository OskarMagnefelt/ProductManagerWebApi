using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_manager_webapi.DTOs.CategoryDtos
{
    public class AddCategoryRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}