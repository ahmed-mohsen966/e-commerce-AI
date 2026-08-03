using AutoMapper;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Addresses.Commands;

public sealed record AddAddressCommand(
    Guid CustomerId,
    AddressType Type,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault) : IRequest<AddressDto>;

public sealed class AddAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<AddAddressCommand, AddressDto>
{
    public async Task<AddressDto> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await unitOfWork.Repository<Customer>()
            .Query()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

        var address = customer.AddAddress(
            request.Type,
            request.Line1,
            request.Line2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.IsDefault);

        // customer is already tracked (loaded above), so a brand-new Address discovered via
        // collection fixup needs to be explicitly Added — see the same note in
        // AddToCartCommandHandler for why EF can't infer Added vs Modified from client-generated
        // GUID keys on an already-tracked parent's collection.
        await unitOfWork.Repository<Address>().AddAsync(address, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<AddressDto>(address);
    }
}
