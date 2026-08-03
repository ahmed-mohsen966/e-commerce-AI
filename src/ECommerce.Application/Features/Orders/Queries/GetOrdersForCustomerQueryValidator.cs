using FluentValidation;

namespace ECommerce.Application.Features.Orders.Queries;

public sealed class GetOrdersForCustomerQueryValidator : AbstractValidator<GetOrdersForCustomerQuery>
{
    public GetOrdersForCustomerQueryValidator() => RuleFor(x => x.CustomerId).NotEmpty();
}
