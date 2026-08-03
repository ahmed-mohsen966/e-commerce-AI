namespace ECommerce.Application.Features.Orders.Dtos;

public sealed class OrderItemDto
{
    public Guid ProductVariantId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPriceAmount { get; init; }
    public string UnitPriceCurrency { get; init; } = string.Empty;
    public decimal LineTotalAmount { get; init; }
    public string LineTotalCurrency { get; init; } = string.Empty;
}
