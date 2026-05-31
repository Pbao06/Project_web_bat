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
                        Name = "Yonex Astrox 100ZZ",
                        Price = 4500000,
                        Image = "80f929d8-65e0-4d0d-8eed-68db75cfafe6.jfif",
                        CategoryId = racketCat,
                        Stock = 15,
                        Brand = "Yonex",
                        Description = "The Astrox 100ZZ is the flagship model of the Astrox series, designed for advanced players.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Victor Thruster F Claw",
                        Price = 3800000,
                        Image = "87aec0e0-7e74-43cf-81a1-556ae80f45a6.jfif",
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
                        Image = "00366836-ff66-4c8a-81c8-63703e9fe13d.jfif",
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
                        Image = "0fa366d8-c9d7-4980-89eb-b58ef0063f43.webp",
                        CategoryId = 6, // Accessories/String
                        Stock = 100,
                        Brand = "Yonex",
                        Description = "High-quality hybrid string for maximum spin and control.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Mizuno Wave Claw 2",
                        Price = 2900000,
                        Image = "73f8b8cd-cf6f-47dd-ad46-cdbf3cca3391.webp",
                        CategoryId = shoeCat,
                        Stock = 12,
                        Brand = "Mizuno",
                        Description = "Professional badminton shoes with excellent cushioning.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Yonex Power Cushion 65Z3",
                        Price = 3100000,
                        Image = "9be9220d-9cad-4778-a251-61f124655042.webp",
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
                        Image = "c18f8ea5-897d-4d6d-a542-2cd68c6adf41.png",
                        CategoryId = shuttleCat,
                        Stock = 50,
                        Brand = "Victor",
                        Description = "Premium goose feather shuttlecocks for tournament use.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Yonex Team Bag 3-Pack",
                        Price = 1200000,
                        Image = "1a432aa5-146c-4ad9-a364-c4cbb9b6c49d.webp",
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