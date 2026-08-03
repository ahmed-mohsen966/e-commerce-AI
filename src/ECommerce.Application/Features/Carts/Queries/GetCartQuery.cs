using AutoMapper;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Carts.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carts.Queries;

public sealed record GetCartQuery(Guid CustomerId) : IRequest<CartDto>;

public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>()
            .Query()
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

        // No cart yet is a normal state for a customer who hasn't added anything —
        // return an empty cart rather than a 404.
        if (cart is null)
        {
            return new CartDto
            {
                Id = Guid.Empty,
                CustomerId = request.CustomerId,
                Items = [],
                TotalAmount = 0,
                TotalCurrency = "USD",
            };
        }

        return _mapper.Map<CartDto>(cart);
    }
}
