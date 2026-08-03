using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Commands;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    Guid ShippingAddressId,
    Guid BillingAddressId) : IRequest<Guid>;

public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = Guid.Empty;

        await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var customer = await _unitOfWork.Repository<Customer>()
                .Query()
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct)
                ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

            if (!customer.Addresses.Any(a => a.Id == request.ShippingAddressId))
                throw new NotFoundException("Address", request.ShippingAddressId);

            if (!customer.Addresses.Any(a => a.Id == request.BillingAddressId))
                throw new NotFoundException("Address", request.BillingAddressId);

            var cartRepository = _unitOfWork.Repository<Cart>();
            var cart = await cartRepository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, ct);

            var variantIds = cart?.Items.Select(i => i.ProductVariantId).ToList() ?? [];

            var variants = await _unitOfWork.Repository<ProductVariant>()
                .Query()
                .Where(v => variantIds.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id, ct);

            var productIds = variants.Values.Select(v => v.ProductId).Distinct().ToList();
            var products = await _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);

            var lines = new List<OrderLineRequest>();

            foreach (var item in cart?.Items ?? [])
            {
                if (!variants.TryGetValue(item.ProductVariantId, out var variant))
                    throw new NotFoundException(nameof(ProductVariant), item.ProductVariantId);

                if (variant.StockQuantity < item.Quantity)
                {
                    throw new DomainException(
                        $"Insufficient stock for SKU '{variant.Sku}'. Requested {item.Quantity}, available {variant.StockQuantity}.");
                }

                // Snapshot the CURRENT variant price at order time — not the cart's
                // possibly-stale price from whenever the item was added. Cloned (not passed
                // by reference) because Money is an EF owned type tracked by CLR reference;
                // reusing variant.Price directly would make EF try to track the same Money
                // instance as owned by both ProductVariant.Price and OrderItem.UnitPrice.
                var productName = products[variant.ProductId].Name;
                var unitPrice = Money.Create(variant.Price.Amount, variant.Price.Currency);
                lines.Add(new OrderLineRequest(variant.Id, productName, variant.Sku, item.Quantity, unitPrice));
            }

            // Order.Place raises OrderPlacedEvent (with these lines) and throws if lines is
            // empty — reusing the aggregate's own "cart must not be empty" validation.
            var order = Order.Place(request.CustomerId, request.ShippingAddressId, request.BillingAddressId, lines);
            await _unitOfWork.Repository<Order>().AddAsync(order, ct);

            cart?.Clear();

            orderId = order.Id;

            // Single SaveChanges: dispatches OrderPlacedEvent pre-commit (the stock-decrement
            // handler mutates the already-tracked ProductVariant entities), then persists the
            // order, the cleared cart, and the stock decrements together atomically.
            await _unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        return orderId;
    }
}
