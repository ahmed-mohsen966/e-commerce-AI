using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Events;

public sealed class OrderPlacedEvent(
    Guid orderId,
    Guid customerId,
    Money totalAmount,
    IReadOnlyCollection<OrderLineRequest> lines) : BaseEvent
{
    public Guid OrderId { get; } = orderId;
    public Guid CustomerId { get; } = customerId;
    public Money TotalAmount { get; } = totalAmount;
    public IReadOnlyCollection<OrderLineRequest> Lines { get; } = lines;
}
