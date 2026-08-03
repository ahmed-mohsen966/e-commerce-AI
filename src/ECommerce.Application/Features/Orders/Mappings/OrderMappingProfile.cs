using AutoMapper;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Orders.Mappings;

public sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Expressed as LINQ over the Items navigation (not the C# TotalAmount computed
        // property) so it's translatable by ProjectTo, not just in-memory Map.
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.Items.Sum(i => i.UnitPrice.Amount * i.Quantity)))
            .ForMember(d => d.TotalCurrency, opt => opt.MapFrom(
                s => s.Items.Select(i => i.UnitPrice.Currency).FirstOrDefault() ?? "USD"));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.Items.Sum(i => i.UnitPrice.Amount * i.Quantity)))
            .ForMember(d => d.TotalCurrency, opt => opt.MapFrom(
                s => s.Items.Select(i => i.UnitPrice.Currency).FirstOrDefault() ?? "USD"))
            .ForMember(d => d.ItemCount, opt => opt.MapFrom(s => s.Items.Count));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.UnitPriceAmount, opt => opt.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.UnitPriceCurrency, opt => opt.MapFrom(s => s.UnitPrice.Currency))
            .ForMember(d => d.LineTotalAmount, opt => opt.MapFrom(s => s.LineTotal.Amount))
            .ForMember(d => d.LineTotalCurrency, opt => opt.MapFrom(s => s.LineTotal.Currency));
    }
}
