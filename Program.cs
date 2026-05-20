using Getdata1.Data;
using Getdata1.Models;
using Getdata1.Services.Implementations;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Getdata1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. DbContext đăng kí cho database 
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Identity — đã có AddIdentity thì KHÔNG cần AddAuthentication riêng
            // bên dưới là đăng kí dịch vụ cho identity 
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                //  update intend for password 
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // 3. Cookie config cho Identity
            builder.Services.ConfigureApplicationCookie(options =>
            {

                options.LoginPath = "/User/Account/Login";
                options.AccessDeniedPath = "/User/Account/AccessDenied";
            });

            // 4. AutoMapper & Custom Services
            builder.Services.AddAutoMapper(typeof(Getdata1.Mappings.MappingProfile));

            // đăng kí gọi service ( hợp long nhất thể interface , và thg culi ) là 1 
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // 6. Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();

            // ✅ Build sau khi đăng ký HẾT services
            var app = builder.Build();

            // Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDeveloperExceptionPage(); // ← cái này hiện lỗi chi tiết trên browser
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.UseAuthentication(); // ✅ sau UseRouting . 1 kiểm tra who are u ( read Cookie)
            app.UseAuthorization();  // ✅ sau UseAuthentication 2. Ông có được vào đây không 

            app.MapStaticAssets();

            // Area route
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            // Default route → vào User/Account/Login
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}/{id?}",
                defaults: new { area = "User", controller = "Cart", action = "Payment"}
            );

            // Seed
            await DbInitializer.SeedAsync(app.Services);

            app.Run();
        }
    }
}
