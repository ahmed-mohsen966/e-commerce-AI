using AutoMapper;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Carts.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carts.Commands;

public sealed record UpdateCartItemCommand(Guid CustomerId, Guid ProductVariantId, int Quantity) : IRequest<CartDto>;

public sealed class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartRepository = _unitOfWork.Repository<Cart>();
        var cart = await cartRepository.Query()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cart), request.CustomerId);

        var variant = await _unitOfWork.Repository<ProductVariant>()
            .GetByIdAsync(request.ProductVariantId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductVariant), request.ProductVariantId);

        if (request.Quantity > variant.StockQuantity)
        {
            throw new DomainException(
                $"Insufficient stock for SKU '{variant.Sku}'. Requested {request.Quantity}, available {variant.StockQuantity}.");
        }

        cart.UpdateItemQuantity(request.ProductVariantId, request.Quantity);

        // cart is already tracked (loaded above); no Update() call needed.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CartDto>(cart);
    }
}
