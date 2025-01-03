using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_manager_webapi.DTOs.ProductDtos
{
    public class ProductDto
    {
        public string Name { get; set; }

        public string SKU { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }
    }
}