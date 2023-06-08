using Indigo.BuisnessLogic.Utilities;
using Indigo.DataAccess.Data;
using Indigo.Models.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace Indigo.BuisnessLogic.Services.DbSeeder
{
    public static class DbSeeder
    {


        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


                await dbContext.Database.EnsureCreatedAsync();


                if (!dbContext.Roles.Any())
                {
                    // Seed roles
                    var roleAdmin = new IdentityRole { Name = StaticDetails.SD_Roles_Admin };
                    var roleUser = new IdentityRole { Name = StaticDetails.SD_Roles_User };
                    await roleManager.CreateAsync(roleAdmin);
                    await roleManager.CreateAsync(roleUser);
                }

                if (!dbContext.Users.Any())
                {
                    // Create Admin user
                    var user = new ApplicationUser
                    {
                        UserName = "johndoh@mail.com",
                        Email = "Johndoh@mail.com",
                        FirstName = "John",
                        LastName = "Doh",
                        EmailConfirmed = true
                    };







                    var result = await userManager.CreateAsync(user, "password");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, StaticDetails.SD_Roles_Admin);
                    }
                }

                await dbContext.SaveChangesAsync();
            }

        }

    }



}

