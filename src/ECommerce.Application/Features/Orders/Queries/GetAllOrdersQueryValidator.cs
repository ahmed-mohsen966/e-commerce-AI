using ECommerce.Domain.Enums;
using FluentValidation;

namespace ECommerce.Application.Features.Orders.Queries;

public sealed class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
{
    public GetAllOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}
