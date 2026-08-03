using AutoMapper;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Addresses.Mappings;

public sealed class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<Address, AddressDto>();
    }
}
