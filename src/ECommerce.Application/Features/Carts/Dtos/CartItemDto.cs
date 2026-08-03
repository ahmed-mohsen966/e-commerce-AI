namespace ECommerce.Application.Features.Carts.Dtos;

public sealed class CartItemDto
{
    public Guid ProductVariantId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPriceAmount { get; init; }
    public string UnitPriceCurrency { get; init; } = string.Empty;
    public decimal LineTotalAmount { get; init; }
    public string LineTotalCurrency { get; init; } = string.Empty;
}
