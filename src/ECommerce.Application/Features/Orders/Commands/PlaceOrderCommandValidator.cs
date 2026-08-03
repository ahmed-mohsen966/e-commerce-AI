using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands;

public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ShippingAddressId).NotEmpty();
        RuleFor(x => x.BillingAddressId).NotEmpty();
    }
}
