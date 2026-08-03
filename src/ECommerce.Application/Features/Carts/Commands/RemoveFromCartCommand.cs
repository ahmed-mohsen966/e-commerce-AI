using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carts.Commands;

public sealed record RemoveFromCartCommand(Guid CustomerId, Guid ProductVariantId) : IRequest;

public sealed class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromCartCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var cartRepository = _unitOfWork.Repository<Cart>();
        var cart = await cartRepository.Query()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cart), request.CustomerId);

        cart.RemoveItem(request.ProductVariantId);

        // cart is already tracked (loaded above); no Update() call needed.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
