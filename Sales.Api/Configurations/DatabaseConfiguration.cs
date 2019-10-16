using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.Infrastructure.Data;

namespace Sales.Api.Configurations
{
    public static class DatabaseExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var salesContext = serviceScope.ServiceProvider.GetRequiredService<SalesContext>();
                salesContext.Database.Migrate();
            }
        }
    }
}
