using FluentValidation;

namespace Sales.Api.ViewModels.Validators
{
    public class ProductCreationValidator : AbstractValidator<ProductCreationViewModel>
    {
        public ProductCreationValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(10).WithName("Product Name")
                .WithMessage("Please specify a {PropertyName}, And the length should be less than {MaximumLength}");
            RuleFor(p => p.FullName).MaximumLength(50);
            RuleFor(p => p.EquivalentTon).GreaterThan(0).WithMessage("{PropertyName} should greater than {GreaterThan}");
            RuleFor(p => p.TaxRate).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} should greater than or equal to {GreaterThan}");
        }
    }
}