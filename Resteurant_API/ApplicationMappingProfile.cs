using AutoMapper;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;

namespace Resteurant_API
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Resteurant, ResteurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(r => r.Adress.City))
                .ForMember(m => m.Street, c => c.MapFrom(r => r.Adress.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(r => r.Adress.PostalCode));

            CreateMap<CreateResteurantDto, Resteurant>()
                .ForMember(r => r.Adress, c => c.MapFrom(dto => new Address() 
                {
                    City = dto.City,
                    Street = dto.Street,
                    PostalCode = dto.PostalCode
                }));

            CreateMap<Dish, DishDto>();
            CreateMap<CreateDishDto, Dish>();
        }
    }
}
