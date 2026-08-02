using ECommerce.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Ensures the Admin/Customer roles exist and seeds a dev-only admin account.
/// Intended to run only in Development — call from Program.cs behind an environment check.
/// </summary>
public static class IdentitySeeder
{
    public const string DevAdminEmail = "admin@ecommerce.local";
    public const string DevAdminPassword = "Admin#12345";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { IdentityRoles.Admin, IdentityRoles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        if (await userManager.FindByEmailAsync(DevAdminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = DevAdminEmail,
            Email = DevAdminEmail,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, DevAdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
    }
}
