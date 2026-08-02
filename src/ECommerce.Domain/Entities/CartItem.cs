using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private CartItem() { }

    internal CartItem(Guid cartId, Guid productVariantId, int quantity, Money unitPrice)
    {
        if (cartId == Guid.Empty)
            throw new DomainException("A cart item must belong to a cart.");
        if (productVariantId == Guid.Empty)
            throw new DomainException("A cart item must reference a product variant.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void UpdatePrice(Money unitPrice)
    {
        UnitPrice = unitPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
