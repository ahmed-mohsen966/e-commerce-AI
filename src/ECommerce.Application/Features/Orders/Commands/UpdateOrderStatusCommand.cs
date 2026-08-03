using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? CancellationReason = null) : IRequest;

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        // Each transition method (re)validates against the aggregate's own state machine —
        // no duplicate transition rules here.
        switch (request.NewStatus)
        {
            case OrderStatus.Paid:
                order.MarkAsPaid();
                break;
            case OrderStatus.Shipped:
                order.Ship();
                break;
            case OrderStatus.Delivered:
                order.Deliver();
                break;
            case OrderStatus.Cancelled:
                order.Cancel(request.CancellationReason ?? string.Empty);
                break;
            default:
                throw new DomainException($"'{request.NewStatus}' is not a valid target status.");
        }

        // order is already tracked (loaded above); no Update() call needed.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
