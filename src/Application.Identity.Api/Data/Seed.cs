using System;
using System.Threading.Tasks;
using Application.Identity.Api.Models;
using Application.Identity.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Identity.Api.Data
{
    public static class Seed
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<IdentitySettings> identitySettings)
        {
            // adding custom roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityResult roleResult;

            foreach (var role in IdentityRoles.GetRoles())
            {
                // creating the roles and seeding them to the database
                var roleExist = await roleManager.RoleExistsAsync(role);

                if (!roleExist)
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
            }

            // creating a super user who could maintain the web app
            var powerUser = new IdentityUser
            {
                UserName = identitySettings.Value.SuperUser.Name,
                Email = identitySettings.Value.SuperUser.Email,
                EmailConfirmed = true
            };

            string userPassword = identitySettings.Value.SuperUser.Password;

            var user = await userManager.FindByEmailAsync(identitySettings.Value.SuperUser.Email);

            if(user == null)
            {
                var createPowerUser = await userManager.CreateAsync(powerUser, userPassword);

                if (createPowerUser.Succeeded)
                {
                    // here we tie new user to the "Admin" role
                    await userManager.AddToRoleAsync(powerUser, IdentityRoles.Admin);
                }
            }
        }
    }
}