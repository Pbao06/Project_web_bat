using Getdata1.Models;
using Getdata1.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Getdata1.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // 1. Ensure Database is created and migrations are applied
            await context.Database.MigrateAsync();

            // 2. Seed Roles
            string[] roles = { "Admin", "Customer" }; // create role for Admin & customer
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }

            }


            // CREATE ACCOUNT USER TO TEST 
            var userEmail = "Ronaldo07@gmail.com";
            // check ton tai 
            var taikhoanUser= await userManager.FindByNameAsync(userEmail);
            if(taikhoanUser == null)
            {
                // tao new user contains information -> User 
                var newUser = new Getdata1.Models.User
                {
                    // map du lieu vao 
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                    Role = UserRole.Customer
                };
                // save to Database
                var saveUser = await userManager.CreateAsync(newUser, "CristianoRonaldo"); // luu thong tin email , password 
                if(saveUser != null && saveUser.Succeeded)
                {
                    // call identity auth for it 
                    await userManager.AddToRoleAsync(newUser, "Customer"); // gan cho no la customer 
                }
            }
           
            
                
            // 3. Seed Admin User
            // 3 way to declare admin -> create email for admin 
            // check exist if not -> create new user -> map info admin -> save to db(email,password) 
            // call identity to auth for them 


            var adminEmail = "Pbao06@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Role = UserRole.Admin // this my enums i created 
                };
                var result = await userManager.CreateAsync(adminUser, "Phangiabao"); // phangiabao is password 
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin"); // identity of microsoft 
                }
            }

            // 4. Seed initial Categories if empty or missing
            var existingCategories = await context.Categories.ToListAsync();
            var categoryNames = existingCategories.Select(c => c.Name.ToLower()).ToList();
            
            // Danh sách các danh mục chuẩn (Tiếng Việt)
            var categoriesToSeed = new List<string> { "Vợt cầu lông", "Quả cầu lông", "Giày cầu lông", "Quần áo", "Túi / Balo", "Phụ kiện" };
            // id lần lượt cho catolory khi mới map qua là 4,5,6,7,8,8 => vợt=4, cầu =5 , giày 6 , túi = 8
            bool changed = false;
            foreach (var catName in categoriesToSeed)
            {
                if (!categoryNames.Contains(catName.ToLower()))
                {
                    context.Categories.Add(new Category { Name = catName });
                    changed = true;
                }
            }
            
            if (changed)
            {
                await context.SaveChangesAsync();
            }

            // (Optional) Handle consolidation if there are English names in existing DB
            // This is safer to do here if the user wants to merge them automatically
            var rackets = existingCategories.Where(c => c.Name.ToLower() == "racket" || c.Name.ToLower() == "rackets").FirstOrDefault();
            var vietRacket = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Vợt cầu lông");
            if (rackets != null && vietRacket != null)
            {
                var productsToMove = await context.Products.Where(p => p.CategoryId == rackets.Id).ToListAsync();
                foreach (var p in productsToMove) p.CategoryId = vietRacket.Id;
                context.Categories.Remove(rackets);
                changed = true;
            }

            var shoes = existingCategories.Where(c => c.Name.ToLower() == "shoes" || c.Name.ToLower() == "footwear").FirstOrDefault();
            var vietShoes = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Giày cầu lông");
            if (shoes != null && vietShoes != null)
            {
                var productsToMove = await context.Products.Where(p => p.CategoryId == shoes.Id).ToListAsync();
                foreach (var p in productsToMove) p.CategoryId = vietShoes.Id;
                context.Categories.Remove(shoes);
                changed = true;
            }

            var shuttles = existingCategories.Where(c => c.Name.ToLower() == "shuttlecock" || c.Name.ToLower() == "shuttlecocks").FirstOrDefault();
            var vietShuttle = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Quả cầu lông");
            if (shuttles != null && vietShuttle != null)
            {
                var productsToMove = await context.Products.Where(p => p.CategoryId == shuttles.Id).ToListAsync();
                foreach (var p in productsToMove) p.CategoryId = vietShuttle.Id;
                context.Categories.Remove(shuttles);
                changed = true;
            }

            if (changed) await context.SaveChangesAsync();

            // 5. Seed Products if empty
            if (!await context.Products.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var racketCat = categories.FirstOrDefault(c => c.Name == "Vợt cầu lông")?.Id ?? 1;
                var shuttleCat = categories.FirstOrDefault(c => c.Name == "Quả cầu lông")?.Id ?? 2;
                var shoeCat = categories.FirstOrDefault(c => c.Name == "Giày cầu lông")?.Id ?? 3;

                context.Products.AddRange(
                    new Product
                    {
                        Name = "Victor 100ZZ",
                        Price = 4500000,
                        Image = "2b39099d-6791-447e-ae49-fe99fcb887c7.webp",
                        CategoryId = racketCat,
                        Stock = 15,
                        Brand = "Victor",
                        Description = "The Victor designed for advanced players.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Victor Thruster F Claw",
                        Price = 3800000,
                        Image = "19233823-2183-4a05-99f3-9cc9d985ded1.jpg",
                        CategoryId = racketCat,
                        Stock = 10,
                        Brand = "Victor",
                        Description = "The signature racket of Tai Tzu Ying, offering great control and power.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Li-Ning Tectonic 7",
                        Price = 3200000,
                        Image = "134240fe-57cd-449c-be7d-3ee49c6d9bd3.webp",
                        CategoryId = racketCat,
                        Stock = 20,
                        Brand = "Li-Ning",
                        Description = "Built with Tectonic technology for faster shuttlecock rebound.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Yonex Aerobite String",
                        Price = 250000,
                        Image = "String.webp",
                        CategoryId = 6, // Accessories/String
                        Stock = 100,
                        Brand = "Yonex",
                        Description = "High-quality hybrid string for maximum spin and control.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "GEARLOGIC MUSE BACKPACK",
                        Price = 2900000,
                        Image = "fd90fe9a-a1d0-46e8-ba7c-4bb16e8c7e0b.webp",
                        CategoryId = 5,
                        Stock = 12,
                        Brand = "Yonex",
                        Description = "Professional badminton pags with excellent cushioning.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Yonex Power Cushion 65Z3",
                        Price = 3100000,
                        Image = "c3c18144-2774-4202-9d22-49632b18f47e.webp",
                        CategoryId = shoeCat,
                        Stock = 8,
                        Brand = "Yonex",
                        Description = "The choice of many top pros for its all-around performance.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Victor AS-30 Shuttlecock",
                        Price = 450000,
                        Image = "1a432aa5-146c-4ad9-a364-c4cbb9b6c49d.webp",
                        CategoryId = shuttleCat,
                        Stock = 50,
                        Brand = "Yonex",
                        Description = "Premium goose feather shuttlecocks for tournament use.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Yonex Team Bag 3-Pack",
                        Price = 1200000,
                        Image = "991044db-b780-448f-b001-001fb8f88da6.webp",
                        CategoryId = 5, // Bags
                        Stock = 25,
                        Brand = "Yonex",
                        Description = "Durable and stylish bag for your essential gear.",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}