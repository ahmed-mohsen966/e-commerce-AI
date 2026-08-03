using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries;

public sealed record GetAllOrdersQuery(
    int PageNumber = 1,
    int PageSize = 20,
    OrderStatus? Status = null) : IRequest<PaginatedList<OrderSummaryDto>>;

public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PaginatedList<OrderSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<PaginatedList<OrderSummaryDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Order>().Query().AsNoTracking();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        query = query.OrderByDescending(o => o.OrderDate);

        var projected = query.ProjectTo<OrderSummaryDto>(_mapper.ConfigurationProvider);

        return PaginatedList<OrderSummaryDto>.CreateAsync(
            projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
