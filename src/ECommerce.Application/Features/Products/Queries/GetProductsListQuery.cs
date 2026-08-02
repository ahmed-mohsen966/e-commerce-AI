using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Queries;

public sealed record GetProductsListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    ProductSortBy SortBy = ProductSortBy.CreatedAt,
    bool SortDescending = true) : IRequest<PaginatedList<ProductListItemDto>>;

public sealed class GetProductsListQueryHandler
    : IRequestHandler<GetProductsListQuery, PaginatedList<ProductListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<PaginatedList<ProductListItemDto>> Handle(
        GetProductsListQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Product>().Query().AsNoTracking();

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice.Amount >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice.Amount <= request.MaxPrice.Value);

        query = request.SortBy switch
        {
            ProductSortBy.Name => request.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            ProductSortBy.BasePrice => request.SortDescending
                ? query.OrderByDescending(p => p.BasePrice.Amount)
                : query.OrderBy(p => p.BasePrice.Amount),
            _ => request.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
        };

        var projected = query.ProjectTo<ProductListItemDto>(_mapper.ConfigurationProvider);

        return PaginatedList<ProductListItemDto>.CreateAsync(
            projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
