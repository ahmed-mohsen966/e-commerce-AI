using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Orders.Dtos;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid ShippingAddressId { get; init; }
    public Guid BillingAddressId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime OrderDate { get; init; }
    public string? CancellationReason { get; init; }
    public decimal TotalAmount { get; init; }
    public string TotalCurrency { get; init; } = string.Empty;
    public IReadOnlyCollection<OrderItemDto> Items { get; init; } = [];
}
