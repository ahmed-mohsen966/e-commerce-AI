using FluentValidation;

namespace ECommerce.Application.Features.Carts.Queries;

public sealed class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator() => RuleFor(x => x.CustomerId).NotEmpty();
}
