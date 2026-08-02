using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Categories.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Categories.Queries;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyCollection<CategoryDto>>;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, IReadOnlyCollection<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> Handle(
        GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Category>()
            .Query()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
