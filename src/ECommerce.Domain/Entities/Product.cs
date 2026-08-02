using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Money BasePrice { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    private Product() { }

    private Product(string name, string description, Guid categoryId, Money basePrice)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BasePrice = basePrice;
    }

    public static Product Create(string name, string description, Guid categoryId, Money basePrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        if (categoryId == Guid.Empty)
            throw new DomainException("Product must belong to a category.");

        return new Product(name.Trim(), description?.Trim() ?? string.Empty, categoryId, basePrice);
    }

    public ProductVariant AddVariant(string sku, string size, string color, Money price, int stockQuantity)
    {
        if (_variants.Any(v => v.Sku.Equals(sku.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"A variant with SKU '{sku}' already exists for this product.");

        var variant = new ProductVariant(Id, sku, size, color, price, stockQuantity);
        _variants.Add(variant);
        UpdatedAt = DateTime.UtcNow;
        return variant;
    }

    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainException("Variant not found on this product.");

        _variants.Remove(variant);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Product must belong to a category.");

        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBasePrice(Money basePrice)
    {
        BasePrice = basePrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
