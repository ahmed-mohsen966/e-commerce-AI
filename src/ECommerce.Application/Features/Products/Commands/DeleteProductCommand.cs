using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Products.Commands;

public sealed record DeleteProductCommand(Guid Id) : IRequest;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Product>();

        var product = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        repository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
