using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto_web_api.Domain.Models;

namespace Proyecto_web_api.Infrastructure.Data
{
    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                try
                {
                    if(!await context.Roles.AnyAsync())
                    {
                        var roles = new[] { "Free", "Premium" };
                        foreach(var roleName in roles)
                        {
                            if (!await roleManager.RoleExistsAsync(roleName))
                            {
                                var role = new Role { Name = roleName };
                                await roleManager.CreateAsync(role);
                            }
                        }
                    }

                    if(!await context.ReactionTypes.AnyAsync())
                    {
                        await context.ReactionTypes.AddRangeAsync(
                            new ReactionType { Name = "Like" }, 
                            new ReactionType { Name = "Love" }, 
                            new ReactionType { Name = "Haha" }, 
                            new ReactionType { Name = "Wow" },
                            new ReactionType { Name = "Sad" } 
                        );
                        await context.SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Un error ha ocurrido mientras se cargaban los seeders");
                    throw;
                }
            }
        }
    }
}