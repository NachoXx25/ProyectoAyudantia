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

                try
                {
                    if(!await context.Roles.AnyAsync())
                    {
                        await context.Roles.AddRangeAsync(
                          new Role { Name = "Free" },
                          new Role { Name = "Premium" }
                        );
                        await context.SaveChangesAsync();
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