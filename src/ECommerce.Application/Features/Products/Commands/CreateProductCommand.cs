using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    Guid CategoryId,
    decimal BasePriceAmount,
    string BasePriceCurrency) : IRequest<Guid>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _unitOfWork.Repository<Category>()
            .Query()
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var basePrice = Money.Create(request.BasePriceAmount, request.BasePriceCurrency);
        var product = Product.Create(request.Name, request.Description, request.CategoryId, basePrice);

        await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
