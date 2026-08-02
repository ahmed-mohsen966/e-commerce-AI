using AutoMapper;
using ECommerce.Application.Features.Categories.Dtos;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Categories.Mappings;

public sealed class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
