using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.Api.ViewModels;
using Sales.Api.ViewModels.Validators;

namespace Sales.Api.Configurations
{
    public static class FluetValidationsConfiguration
    {
        public static void AddFluetValidations(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProductCreationValidator>());
        }
    }
}
