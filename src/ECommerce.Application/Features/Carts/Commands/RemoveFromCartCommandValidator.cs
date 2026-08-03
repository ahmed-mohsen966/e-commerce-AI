using FluentValidation;

namespace ECommerce.Application.Features.Carts.Commands;

public sealed class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ProductVariantId).NotEmpty();
    }
}
