using AutoMapper;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Products.Mappings;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.BasePriceAmount, opt => opt.MapFrom(s => s.BasePrice.Amount))
            .ForMember(d => d.BasePriceCurrency, opt => opt.MapFrom(s => s.BasePrice.Currency));

        CreateMap<Product, ProductListItemDto>()
            .ForMember(d => d.BasePriceAmount, opt => opt.MapFrom(s => s.BasePrice.Amount))
            .ForMember(d => d.BasePriceCurrency, opt => opt.MapFrom(s => s.BasePrice.Currency));

        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(d => d.PriceAmount, opt => opt.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.PriceCurrency, opt => opt.MapFrom(s => s.Price.Currency));
    }
}
