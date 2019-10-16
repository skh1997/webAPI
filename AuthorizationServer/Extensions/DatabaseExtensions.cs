using System.Threading.Tasks;
using AuthorizationServer.Data;
using AuthorizationServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationServer.Extensions
{
    public static class DatabaseExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                
                applicationDbContext.Database.Migrate();
                persistedGrantDbContext.Database.Migrate();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                Task.Run(async () =>
                {
                    var admin = await userManager.FindByNameAsync("admin");
                    if (admin == null)
                    {
                        await userManager.CreateAsync(new ApplicationUser
                        {
                            UserName = "admin",
                            Email = "emailyx@gmail.com"
                        }, "admin");
                    }
                }).GetAwaiter().GetResult();
                
            }
        }
    }
}
