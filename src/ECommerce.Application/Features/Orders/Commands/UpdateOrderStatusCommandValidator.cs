using ECommerce.Domain.Enums;
using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .When(x => x.NewStatus == OrderStatus.Cancelled)
            .WithMessage("A cancellation reason is required when cancelling an order.");
    }
}
