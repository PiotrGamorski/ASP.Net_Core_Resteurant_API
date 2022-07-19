using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resteurant_API.Interfaces
{
    public interface IDishService
    {
        Task Update(int resteurantId, int dishId, UpdateDishDto dto);
        Task Delete(int resteurantId, int dishId);
        Task<Dish> Create(int resteurantId, CreateDishDto dto);
        Task<IEnumerable<DishDto>> GetAll(int resteurantId);
        Task<DishDto> GetById(int resteurnatId, int dishId);
    }
}
