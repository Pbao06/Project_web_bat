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

            // 4. Seed initial Categories if empty
            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "Rackets" },
                    new Category { Name = "Shuttlecocks" },
                    new Category { Name = "Shoes" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}