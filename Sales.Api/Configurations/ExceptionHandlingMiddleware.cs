using System;
using System.Net;
using System.Text;
using ApiSettings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Sales.Api.ViewModels;
using Sales.Api.ViewModels.Common;
using Sales.Infrastructure.UsefulModels;

namespace Sales.Api.Configurations
{
    public static class ExceptionHandlingMiddleware
    {
        public static void UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
            {
                options.UseCors(SalesApiSettings.CorsPolicyName);
                options.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            var err = new ErrorViewModel
                            {
                                StackTrace = ex.Error.StackTrace,
                                Message = ex.Error.Message,
                                InnerMessage = GetInnerMessage(ex.Error)
                            };
                            var errStr = JsonConvert.SerializeObject(err, Formatting.None,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                                });
                            var bytes = Encoding.UTF8.GetBytes(errStr);
                            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                        }
                    });
            });
        }

        private static string GetInnerMessage(Exception ex)
        {
            if (ex == null) return null;
            return ex.InnerException != null ? GetInnerMessage(ex.InnerException) : ex.Message;
        }
    }
}
