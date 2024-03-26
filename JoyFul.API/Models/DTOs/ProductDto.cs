using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace JoyFul.API.Models.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; }

        [MaxLength(100)]
        public string Brand { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
