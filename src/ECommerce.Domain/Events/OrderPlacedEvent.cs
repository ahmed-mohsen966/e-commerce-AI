using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Events;

public sealed class OrderPlacedEvent(Guid orderId, Guid customerId, Money totalAmount) : BaseEvent
{
    public Guid OrderId { get; } = orderId;
    public Guid CustomerId { get; } = customerId;
    public Money TotalAmount { get; } = totalAmount;
}
