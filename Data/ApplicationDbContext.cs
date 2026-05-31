using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Getdata1.Models;


namespace Getdata1.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } 
        public DbSet<Category> Categories { get; set; }  
        public DbSet<_Order> _Orders { get; set; }  
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductsImage> ProductsImages { get; set; } 
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình decimal cho toàn bộ các thuộc tính tiền tệ
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // _Order Status enum → string
            modelBuilder.Entity<_Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            // User Role enum → string
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}