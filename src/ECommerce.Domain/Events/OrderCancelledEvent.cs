using ECommerce.Domain.Common;

namespace ECommerce.Domain.Events;

public sealed class OrderCancelledEvent(Guid orderId, Guid customerId, string reason) : BaseEvent
{
    public Guid OrderId { get; } = orderId;
    public Guid CustomerId { get; } = customerId;
    public string Reason { get; } = reason;
}
