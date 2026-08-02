using FluentValidation;

namespace ECommerce.Application.Features.Products.Queries;

public sealed class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
