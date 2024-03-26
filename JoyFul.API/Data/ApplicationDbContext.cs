using JoyFul.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JoyFul.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
