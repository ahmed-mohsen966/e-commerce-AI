using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Commands;

public sealed record CreateProductVariantCommand(
    Guid ProductId,
    string Sku,
    string Size,
    string Color,
    decimal PriceAmount,
    string PriceCurrency,
    int StockQuantity) : IRequest<Guid>;

public sealed class CreateProductVariantCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductVariantCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.Repository<Product>();

        // Included so Product.AddVariant's own duplicate-SKU-within-product check actually
        // has data to check against (GetByIdAsync alone wouldn't load Variants).
        var product = await productRepository.Query()
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        var price = Money.Create(request.PriceAmount, request.PriceCurrency);
        var variant = product.AddVariant(request.Sku, request.Size, request.Color, price, request.StockQuantity);

        // product is already tracked (loaded above), so its own mutations are picked up
        // automatically — but the new ProductVariant, discovered via collection fixup on an
        // already-tracked parent, must be explicitly Added. EF's Added-vs-Modified heuristic
        // can't tell a new child from an existing one when keys are client-generated GUIDs
        // (never "default"), so without this it issues an UPDATE for a row that doesn't
        // exist yet (DbUpdateConcurrencyException).
        await unitOfWork.Repository<ProductVariant>().AddAsync(variant, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return variant.Id;
    }
}
