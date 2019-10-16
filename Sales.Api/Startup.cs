using System.Text;
using ApiSettings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Sales.Api.Configurations;
using Sales.Infrastructure.Data;
using Swashbuckle.AspNetCore.Swagger;

namespace Sales.Api
{
    public class Startup
    {
        private IServiceCollection _services;
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SalesContext>(options => options.UseSqlServer(Configuration["SalesApi:DefaultConnection"]));
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                // set authorization on all controllers or routes
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluetValidations();

            services.AddAutoMapper();
            services.AddServices();
            services.AddRepositories();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = SalesApiSettings.ApiDisplayName, Version = "v1" });
            });

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["AuthorizationServer:ServerBase"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = SalesApiSettings.ApiName;
                });

            services.AddCors(options =>
            {
                options.AddPolicy(SalesApiSettings.CorsPolicyName, policy =>
                {
                    policy.WithOrigins(Configuration["SalesApi:ClientBase"])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            _services = services;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                ListAllRegisteredServices(app);
            }
            else
            {
                app.InitializeDatabase();
            }
            app.UseExceptionHandlingMiddleware();
            app.UseCors(SalesApiSettings.CorsPolicyName);
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", SalesApiSettings.ClientName + " API v1");
            });
            app.UseAuthentication();
            app.UseMvc();
        }

        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}
