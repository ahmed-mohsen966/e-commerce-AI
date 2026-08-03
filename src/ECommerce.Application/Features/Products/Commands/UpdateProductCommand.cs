using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    decimal BasePriceAmount,
    string BasePriceCurrency,
    bool IsActive) : IRequest;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (request.CategoryId != product.CategoryId)
        {
            var categoryExists = await _unitOfWork.Repository<Category>()
                .Query()
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (!categoryExists)
                throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        product.UpdateDetails(request.Name, request.Description);
        product.ChangeCategory(request.CategoryId);
        product.UpdateBasePrice(Money.Create(request.BasePriceAmount, request.BasePriceCurrency));

        if (request.IsActive)
            product.Activate();
        else
            product.Deactivate();

        // product is already tracked (loaded above); no Update() call needed.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
