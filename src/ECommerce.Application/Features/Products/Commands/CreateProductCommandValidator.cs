using FluentValidation;

namespace ECommerce.Application.Features.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotNull().MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BasePriceAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BasePriceCurrency).NotEmpty().Length(3);
    }
}
