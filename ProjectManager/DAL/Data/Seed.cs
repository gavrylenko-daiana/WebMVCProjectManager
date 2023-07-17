using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Data;

public class Seed
{
    public static async Task SeedData(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("StakeHolder"))
                await roleManager.CreateAsync(new IdentityRole("StakeHolder"));

            if (!await roleManager.RoleExistsAsync("Developer"))
                await roleManager.CreateAsync(new IdentityRole("Developer"));

            if (!await roleManager.RoleExistsAsync("Tester"))
                await roleManager.CreateAsync(new IdentityRole("Tester"));
        }
    }
}