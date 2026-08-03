using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using MediatR;

namespace ECommerce.Application.Features.Orders.EventHandlers;

/// <summary>
/// Decrements stock for each ordered variant. Runs as part of the same SaveChanges call that
/// persists the order (see ApplicationDbContext's pre-commit domain event dispatch), so the
/// stock decrement and the order creation succeed or fail together atomically.
/// </summary>
public sealed class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderPlacedEventHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ProductVariant>();

        foreach (var line in notification.Lines)
        {
            var variant = await repository.GetByIdAsync(line.ProductVariantId, cancellationToken)
                ?? throw new NotFoundException(nameof(ProductVariant), line.ProductVariantId);

            variant.Reserve(line.Quantity);
        }
    }
}
