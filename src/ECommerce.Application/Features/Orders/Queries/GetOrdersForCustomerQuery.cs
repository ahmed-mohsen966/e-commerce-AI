using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries;

public sealed record GetOrdersForCustomerQuery(Guid CustomerId) : IRequest<IReadOnlyCollection<OrderSummaryDto>>;

public sealed class GetOrdersForCustomerQueryHandler
    : IRequestHandler<GetOrdersForCustomerQuery, IReadOnlyCollection<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrdersForCustomerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<OrderSummaryDto>> Handle(
        GetOrdersForCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Order>()
            .Query()
            .AsNoTracking()
            .Where(o => o.CustomerId == request.CustomerId)
            .OrderByDescending(o => o.OrderDate)
            .ProjectTo<OrderSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
