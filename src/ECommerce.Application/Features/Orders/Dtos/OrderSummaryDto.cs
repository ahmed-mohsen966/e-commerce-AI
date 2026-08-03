using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Orders.Dtos;

public sealed class OrderSummaryDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string TotalCurrency { get; init; } = string.Empty;
    public int ItemCount { get; init; }
}
