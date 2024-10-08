using DATN_SalesWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Repository
{
    public class SeedData
    {
        public static async Task SeedingData(DataContext _context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            await _context.Database.MigrateAsync();

            // Kiểm tra và tạo Role ADMIN nếu chưa tồn tại
            if (await roleManager.FindByNameAsync("ADMIN") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }

            // Kiểm tra và tạo Role BUYER nếu chưa tồn tại
            if (await roleManager.FindByNameAsync("BUYER") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("BUYER"));
            }

            // Kiểm tra dữ liệu mẫu cho Product nếu chưa tồn tại
            if (!_context.Products.Any())
            {
                CategoryModel macbook = new CategoryModel { Name = "macbook", Slug = "macbook", Description = "macbook macbook macbook", Status = 1 };
                CategoryModel pc = new CategoryModel { Name = "Pc", Slug = "Pc", Description = "Pc Pc Pc", Status = 1 };
                BrandModel apple = new BrandModel { Name = "Apple", Slug = "Apple", Description = "Apple Apple Apple", Status = 1 };
                BrandModel samsung = new BrandModel { Name = "Desktop", Slug = "Desktop", Description = "Desktop Desktop Desktop", Status = 1 };

                await _context.Products.AddRangeAsync(
                    new ProductModel { Name = "Macbook", Slug = "macbook", Description = "macbook macbook macbook", Image = "1.jpg", Category = macbook, Brand = apple, Price = 1000 },
                    new ProductModel { Name = "Pc", Slug = "Pc", Description = "Pc Pc Pc", Image = "1.jpg", Category = pc, Brand = samsung, Price = 1000 }
                );
                await _context.SaveChangesAsync();
            }

            // Kiểm tra người dùng
            var existingUser = await userManager.FindByNameAsync("infinityaimh9999");
            if (existingUser == null)
            {
                // Nếu người dùng không tồn tại, tạo mới
                var user = new AppUserModel
                {
                    UserName = "infinityaimh9999",
                    Email = "infinityaimh9999@gmail.com",
                    PasswordHash = new PasswordHasher<AppUserModel>().HashPassword(null, "123abc456"),
                };

                var createUserResult = await userManager.CreateAsync(user);

                if (createUserResult.Succeeded)
                {
                    // Gán user vào role ADMIN
                    var adminRole = await roleManager.FindByNameAsync("ADMIN");

                    if (adminRole != null)
                    {
                        await userManager.AddToRoleAsync(user, "ADMIN");
                        user.RoleId = adminRole.Id; // Gán RoleId vào user
                        await userManager.UpdateAsync(user); // Cập nhật user với RoleId mới
                    }
                }
            }
        }
    }
}
