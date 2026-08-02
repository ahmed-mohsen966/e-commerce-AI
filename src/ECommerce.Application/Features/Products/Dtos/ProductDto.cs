namespace ECommerce.Application.Features.Products.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public decimal BasePriceAmount { get; init; }
    public string BasePriceCurrency { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<ProductVariantDto> Variants { get; init; } = [];
}
