using AutoMapper;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Carts.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carts.Commands;

public sealed record AddToCartCommand(Guid CustomerId, Guid ProductVariantId, int Quantity) : IRequest<CartDto>;

public sealed class AddToCartCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<AddToCartCommand, CartDto>
{
    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var variant = await unitOfWork.Repository<ProductVariant>()
            .GetByIdAsync(request.ProductVariantId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductVariant), request.ProductVariantId);

        var cartRepository = unitOfWork.Repository<Cart>();
        var cart = await cartRepository.Query()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

        var isNewCart = cart is null;
        cart ??= Cart.CreateFor(request.CustomerId);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);
        var isNewLine = existingItem is null;
        var resultingQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (resultingQuantity > variant.StockQuantity)
        {
            throw new DomainException(
                $"Insufficient stock for SKU '{variant.Sku}'. Requested {resultingQuantity}, available {variant.StockQuantity}.");
        }

        // Money is an EF owned type tracked by CLR reference; reusing variant.Price directly
        // would make EF try to track the same Money instance as owned by two entities at
        // once (ProductVariant.Price and CartItem.UnitPrice), which corrupts change tracking.
        // Always clone owned-type values when copying them onto another entity.
        var unitPrice = Money.Create(variant.Price.Amount, variant.Price.Currency);
        var item = cart.AddItem(request.ProductVariantId, request.Quantity, unitPrice);

        if (isNewCart)
        {
            // A brand-new root entity: Add() cascades Added state to its whole graph
            // (including the new CartItem), regardless of the CartItem's key value.
            await cartRepository.AddAsync(cart, cancellationToken);
        }
        else if (isNewLine)
        {
            // cart itself is already tracked, so its mutation is picked up automatically —
            // but a brand-new CartItem discovered via collection fixup on an *already*
            // tracked parent needs to be explicitly Added. EF's automatic Added-vs-Modified
            // heuristic decides based on whether the key looks "unset", and our GUID keys
            // are always client-generated (never Guid.Empty), so without this, EF assumes
            // the item already exists and issues an UPDATE for a row that isn't there yet —
            // a spurious DbUpdateConcurrencyException ("expected 1 row, affected 0").
            await unitOfWork.Repository<CartItem>().AddAsync(item, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<CartDto>(cart);
    }
}
