using AutoMapper;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries;

/// <summary>
/// Fetches a single order, scoped to the requesting customer. An order that exists but
/// belongs to someone else is reported as not found, not forbidden — existence of another
/// customer's order is not disclosed.
/// </summary>
public sealed record GetOrderByIdQuery(Guid OrderId, Guid CustomerId) : IRequest<OrderDto>;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>()
            .Query()
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.CustomerId == request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        return _mapper.Map<OrderDto>(order);
    }
}
