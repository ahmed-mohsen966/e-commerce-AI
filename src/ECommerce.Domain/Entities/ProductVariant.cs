using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Size { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }

    private ProductVariant() { }

    internal ProductVariant(Guid productId, string sku, string size, string color, Money price, int stockQuantity)
    {
        if (productId == Guid.Empty)
            throw new DomainException("A product variant must belong to a product.");
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");
        if (string.IsNullOrWhiteSpace(size))
            throw new DomainException("Size is required.");
        if (string.IsNullOrWhiteSpace(color))
            throw new DomainException("Color is required.");
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        ProductId = productId;
        Sku = sku.Trim().ToUpperInvariant();
        Size = size.Trim();
        Color = color.Trim();
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void Reprice(Money newPrice)
    {
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdjustStock(int delta)
    {
        var newQuantity = StockQuantity + delta;
        if (newQuantity < 0)
            throw new DomainException("Insufficient stock for this operation.");

        StockQuantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to reserve must be positive.");

        AdjustStock(-quantity);
    }
}
