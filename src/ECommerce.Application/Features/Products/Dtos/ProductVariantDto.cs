namespace ECommerce.Application.Features.Products.Dtos;

public sealed class ProductVariantDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Size { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; } = string.Empty;
    public int StockQuantity { get; init; }
}
