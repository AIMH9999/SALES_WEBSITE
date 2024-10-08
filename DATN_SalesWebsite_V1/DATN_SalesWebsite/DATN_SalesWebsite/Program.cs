using DATN_SalesWebsite.Areas.Admin.Repository;
using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DATN_SalesWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Connection db
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
            });

            //Email

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromSeconds(30);
                option.Cookie.IsEssential = true;
            });


            builder.Services.AddIdentity<AppUserModel, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
 

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;


                options.User.RequireUniqueEmail = true;
            });


            var app = builder.Build();

            app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");


            app.MapControllerRoute(
                name: "Category",
                pattern: "/Category/{Slug?}",
                defaults: new { controller = "Category", action = "Index"});

            app.MapControllerRoute(
               name: "Brand",
               pattern: "/Brand/{Slug?}",
               defaults: new { controller = "Brand", action = "Index" });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
                var userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUserModel>>();
                var roleManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                SeedData.SeedingData(context, userManager, roleManager);
            }

            app.Run();
        }
    }
}
