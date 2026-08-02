using MediatR;

namespace ECommerce.Domain.Common;

public abstract class BaseEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
