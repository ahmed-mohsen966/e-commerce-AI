using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; private set; }

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    private Cart(Guid customerId)
    {
        CustomerId = customerId;
    }

    public static Cart CreateFor(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("A cart must belong to a customer.");

        return new Cart(customerId);
    }

    public CartItem AddItem(Guid productVariantId, int quantity, Money unitPrice)
    {
        var existing = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId);
        if (existing is not null)
        {
            existing.ChangeQuantity(existing.Quantity + quantity);
            existing.UpdatePrice(unitPrice);
            UpdatedAt = DateTime.UtcNow;
            return existing;
        }

        var item = new CartItem(Id, productVariantId, quantity, unitPrice);
        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
        return item;
    }

    public void RemoveItem(Guid productVariantId)
    {
        var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId)
            ?? throw new DomainException("Item not found in cart.");

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(Guid productVariantId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId)
            ?? throw new DomainException("Item not found in cart.");

        item.ChangeQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        _items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public Money CalculateTotal(string currency = "USD")
    {
        if (_items.Count == 0)
            return Money.Zero(currency);

        return _items.Aggregate(Money.Zero(_items[0].UnitPrice.Currency), (total, item) => total.Add(item.LineTotal));
    }
}
