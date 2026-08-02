using FluentValidation;

namespace ECommerce.Application.Features.Products.Commands;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
