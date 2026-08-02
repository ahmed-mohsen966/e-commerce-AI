using AutoMapper;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>()
            .Query()
            .AsNoTracking()
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return _mapper.Map<ProductDto>(product);
    }
}
