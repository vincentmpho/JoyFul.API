using JoyFul.API.Data;
using JoyFul.API.Models;
using JoyFul.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JoyFul.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAllProducts(string? serach, string? category,
            int? minPrice, int? maxPrice,
            string? sort, string? order
            )
        {
            //serch Functionality
            IQueryable<Product>query = _context.Products;

            if (serach !=null)
            {
                query = query.Where(p => p.Name.Contains(serach) || p.Description.Contains(serach));
            }

            //check if it's null or not
            if (category != null)
            {
                query = query.Where(p=>p.Category==category);
            }

            if(minPrice != null)
            {
                query =query.Where(p=>p.Price<=maxPrice);
            }

            if(maxPrice != null)
            {
                query = query.Where(p=>p.Price<=minPrice);
            }

            //Add the serch criteria

            //Sort Functionality
            if(sort == null)sort = "id";
            if (order == null || order != "asc") order = "desc";

            if (sort.ToLower() == "name")
            {
                if(order == "asc")
                {
                    query = query.OrderBy(p=>p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p=>p.Name);
                }
            }

           else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }


            else if (sort.ToLower() == "category")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }

            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }

            else if (sort.ToLower() == "date")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else 
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //Pagination Functionality
            //if (page == null || page < 1) page = 1;

            //int pageSize = 5;
            //int totalPages = 0;

            ////find the toal number of prudcts
            //decimal count = query.Count();

            ////find the total of pages
            //totalPages =(int)Math.Ceiling(count/pageSize);
            //query = query.Skip((int)page-1 * pageSize).Take(pageSize);

            //List of product
            var products = query.ToList();

           

            if (products == null)
            {
                return NotFound();
            }
            
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProoduct([FromForm] ProductDto productDto)
        {
            if (productDto == null)
            {
                ModelState.AddModelError("ImageFiile", "The Image File is required");
                return BadRequest(ModelState);
            }
            //Save the image on the server
            string imageFileName = DateTime.Now.ToString("yyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

            //contain  the absolute path on the wwwroot folder
            string imageFolder = _env.WebRootPath + "/images/products/";

            //Save images
            using (var stream = System.IO.File.Create(imageFolder + imageFileName))
            {
                //save received image
                productDto.ImageFile.CopyTo(stream);
            }

            //Save Product in the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now,
            };

            //add to databse
            _context.Products.Add(product);
            _context.SaveChanges();

            //save to the client
            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            // Check if we have a valid product in the database with the given id
            var product = _context.Products.FirstOrDefault(x => x.Id == id);

            // Check if the product exists
            if (product == null)
            {
                return NotFound();
            }

            // Read the old product file name
            string imageFileName = product.ImageFileName;

            // Check if we have a new file in the productDto
            if (productDto.ImageFile != null)
            {
                // Save the image on the server with a new file name
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile.FileName);

                string imageFolder = _env.WebRootPath + "/images/products/";

                // Create the new file
                using (var stream = System.IO.File.Create(imageFolder + imageFileName))
                {
                    // Save the received image
                    productDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image
                System.IO.File.Delete(imageFolder + product.ImageFileName);
            }

            // Update the product in the database
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.ImageFileName = imageFileName;
            product.Price = productDto.Price;

            // Save changes
            _context.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            //Delete the image on the server
            string imagesFolder = _env.WebRootPath + "images/products/";
            System.IO.File.Delete(imagesFolder + product.ImageFileName);

            //Delete product from databse
            _context.Products.Remove(product);

            //save
            _context.SaveChanges();

            return Ok();
        }
    }
}
