using Microsoft.EntityFrameworkCore;
using my_api.Models;

namespace my_api.Data
{
    // AppDbContext inherits from DbContext, which is the core EF Core class
    public class AppDbContext : DbContext
    {
        // This constructor is required to pass configuration options 
        // (like the connection string) to the base DbContext class.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // This property represents the 'Users' table in the PostgreSQL database.
        // EF Core will manage the data in this table using your User model.
        public DbSet<User> Users { get; set; }
    }
}