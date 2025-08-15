using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task SeedRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));

        if (!await roleManager.RoleExistsAsync(Roles.Member))
            await roleManager.CreateAsync(new IdentityRole(Roles.Member));
    }
}