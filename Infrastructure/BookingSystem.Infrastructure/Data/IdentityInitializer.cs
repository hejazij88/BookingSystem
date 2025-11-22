using BookingSystem.Domain.Constants;
using BookingSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookingSystem.Infrastructure.Data;

public static class IdentityInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. ساخت نقش‌ها اگر وجود ندارند
        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Admin));

        if (!await roleManager.RoleExistsAsync(ApplicationRoles.Client))
            await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Client));

        // 2. ساخت یک کاربر ادمین پیش فرض
        var adminUser = await userManager.FindByEmailAsync("admin@test.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "System",
                LastName = "Admin"
            };

            await userManager.CreateAsync(adminUser, "Admin123!");

            // 3. انتساب نقش ادمین به این کاربر
            await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
        }
    }
}