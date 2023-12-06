using AutoMapper;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Infrastructures.Identities;

namespace ExpressMeals.WebServer.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Category, CategoryVm>().ReverseMap();

            CreateMap<Meal, MealVm>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<MealVm, Meal>().ReverseMap();

            CreateMap<ApplicationUser, UserVm>().ReverseMap();

            //CreateMap<OrderCreateVm, Order>().ReverseMap();

            //CreateMap<OrderDetail, OrderDetailVm>().ReverseMap();

            //CreateMap<OrderVm, OrderDto>().ReverseMap();
        }
    }
}