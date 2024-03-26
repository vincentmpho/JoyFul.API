using JoyFul.API.Data;
using JoyFul.API.Models;
using JoyFul.API.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JoyFul.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserDto userDto)
        {
            // Check if the provided email address is already in use
            var emailAccount = _context.Users.Count(u => u.Email == userDto.Email);
            if (emailAccount > 0)
            {
                ModelState.AddModelError("Email", "This Email Address is Already Used");
                return BadRequest(ModelState);
            }

            // Encrypt the user's password before storing it in the database
            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new User(), userDto.Password);

            // Create a new User object with provided user data
            User user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone ?? "", // If phone number is null, assign an empty string
                Address = userDto.Address,
                Password = encryptedPassword, // Store encrypted password
                Role = "client", // Set default role to 'client'
                CreatedAt = DateTime.Now, // Set current timestamp as creation time
            };

            // Add the new user to the database
            _context.Users.Add(user);
            _context.SaveChanges(); // Save changes to the database

            // Create a JWT token for the user
            var jwt = CreateJWTToken(user);

            // Create a UserProfileDto object containing user details
            UserProfileDto userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now, 
            };

            // Create a response object containing JWT token and user profile details
            var response = new
            {
                Token = jwt, // JWT token
                User = userProfileDto // User profile details
            };

            // Return HTTP OK response with the response object
            return Ok(response);
        }

        // Method to create a JWT token for the user
        private string CreateJWTToken(User user)
        {
            // Define claims for the JWT token (e.g., user ID, role)
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", "" + user.Id), // User ID claim
                new Claim("role", user.Role), // Role claim
            };

            // Read the secret key from configuration
            string strKey = _configuration["JwtSettings:Key"];

            // Create a SymmetricSecurityKey using the secret key
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strKey));

            // Create signing credentials using the key and HmacSha512 algorithm
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512);

            // Create a JWT token with issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token expiration time (1 day from now)
                signingCredentials: creds
            );

            // Write the JWT token as a string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Return the JWT token
            return jwt;
        }
    }
}
