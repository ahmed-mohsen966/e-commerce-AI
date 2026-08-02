using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public string Sku { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private OrderItem() { }

    internal OrderItem(Guid orderId, Guid productVariantId, string productName, string sku, int quantity, Money unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("An order item must belong to an order.");
        if (productVariantId == Guid.Empty)
            throw new DomainException("An order item must reference a product variant.");
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name is required.");
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        OrderId = orderId;
        ProductVariantId = productVariantId;
        ProductName = productName.Trim();
        Sku = sku.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
