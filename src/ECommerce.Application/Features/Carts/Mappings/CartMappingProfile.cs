using AutoMapper;
using ECommerce.Application.Features.Carts.Dtos;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Carts.Mappings;

public sealed class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.CalculateTotal("USD").Amount))
            .ForMember(d => d.TotalCurrency, opt => opt.MapFrom(s => s.CalculateTotal("USD").Currency));

        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.UnitPriceAmount, opt => opt.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.UnitPriceCurrency, opt => opt.MapFrom(s => s.UnitPrice.Currency))
            .ForMember(d => d.LineTotalAmount, opt => opt.MapFrom(s => s.LineTotal.Amount))
            .ForMember(d => d.LineTotalCurrency, opt => opt.MapFrom(s => s.LineTotal.Currency));
    }
}
