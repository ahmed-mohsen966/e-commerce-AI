using FluentValidation;

namespace ECommerce.Application.Features.Products.Commands;

public sealed class CreateProductVariantCommandValidator : AbstractValidator<CreateProductVariantCommand>
{
    public CreateProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Size).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceCurrency).NotEmpty().Length(3);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
