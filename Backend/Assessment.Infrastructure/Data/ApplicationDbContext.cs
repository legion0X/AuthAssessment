using Assessment.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@gmail.com",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Addmin@123"),
                Role = "Admin"
            });
        }

    }
}
