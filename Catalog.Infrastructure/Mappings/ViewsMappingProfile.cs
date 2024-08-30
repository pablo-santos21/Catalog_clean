using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Identity;

namespace Catalog.Infrastructure.Mappings;

public class ViewsMappingProfile : Profile
{
    public ViewsMappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<ScheduledEvent, EventDTO>().ReverseMap();
        CreateMap<PostBlog, PostDTO>().ReverseMap();
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<CategoryBlog, CategoryBlogDTO>().ReverseMap();
        CreateMap<TypeEvent, TypeEventDTO>().ReverseMap();
        CreateMap<ApplicationUser, IdentityApplicationUser>().ReverseMap();
    }
}
