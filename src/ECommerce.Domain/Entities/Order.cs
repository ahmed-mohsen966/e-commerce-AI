using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Events;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public sealed record OrderLineRequest(Guid ProductVariantId, string ProductName, string Sku, int Quantity, Money UnitPrice);

public class Order : BaseEntity
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Paid, OrderStatus.Cancelled],
        [OrderStatus.Paid] = [OrderStatus.Shipped, OrderStatus.Cancelled],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Cancelled] = [],
    };

    public Guid CustomerId { get; private set; }
    public Guid ShippingAddressId { get; private set; }
    public Guid BillingAddressId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money TotalAmount => _items.Count == 0
        ? Money.Zero()
        : _items.Aggregate(Money.Zero(_items[0].UnitPrice.Currency), (total, item) => total.Add(item.LineTotal));

    private Order() { }

    private Order(Guid customerId, Guid shippingAddressId, Guid billingAddressId)
    {
        CustomerId = customerId;
        ShippingAddressId = shippingAddressId;
        BillingAddressId = billingAddressId;
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
    }

    public static Order Place(Guid customerId, Guid shippingAddressId, Guid billingAddressId, IReadOnlyCollection<OrderLineRequest> lines)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("An order must belong to a customer.");
        if (shippingAddressId == Guid.Empty)
            throw new DomainException("A shipping address is required.");
        if (billingAddressId == Guid.Empty)
            throw new DomainException("A billing address is required.");
        if (lines is null || lines.Count == 0)
            throw new DomainException("An order must contain at least one item.");

        var order = new Order(customerId, shippingAddressId, billingAddressId);

        foreach (var line in lines)
        {
            order._items.Add(new OrderItem(order.Id, line.ProductVariantId, line.ProductName, line.Sku, line.Quantity, line.UnitPrice));
        }

        order.AddDomainEvent(new OrderPlacedEvent(order.Id, order.CustomerId, order.TotalAmount, lines));

        return order;
    }

    public void MarkAsPaid()
    {
        EnsureTransitionAllowed(OrderStatus.Paid);
        Status = OrderStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        EnsureTransitionAllowed(OrderStatus.Shipped);
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        EnsureTransitionAllowed(OrderStatus.Delivered);
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A cancellation reason is required.");

        EnsureTransitionAllowed(OrderStatus.Cancelled);
        Status = OrderStatus.Cancelled;
        CancellationReason = reason.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderCancelledEvent(Id, CustomerId, CancellationReason));
    }

    private void EnsureTransitionAllowed(OrderStatus target)
    {
        if (!AllowedTransitions[Status].Contains(target))
            throw new DomainException($"Cannot transition order from '{Status}' to '{target}'.");
    }
}
