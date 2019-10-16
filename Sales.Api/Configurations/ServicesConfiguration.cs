using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Sales.Core.Interfaces;
using Sales.Infrastructure.Data;
using Sales.Infrastructure.Interfaces;
using Sales.Infrastructure.Services;

namespace Sales.Api.Configurations
{
    public static class ServicesConfiguration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                    .ActionContext;
                return new UrlHelper(actionContext);
            });

            var physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
            services.AddSingleton<IFileProvider>(compositeProvider);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(ICoreService<>), typeof(CoreService<>));
        }
    }
}
