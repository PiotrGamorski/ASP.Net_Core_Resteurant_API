using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Models;
using System.Threading.Tasks;

namespace Resteurant_API.Interfaces
{
    public interface IResteurantService
    {
        Task<ResteurantDto> GetById(int id);
        Task<PagedResult<ResteurantDto>> GetAll(ResteurantQuery query);
        Task<Resteurant> Create(CreateResteurantDto dto);
        Task Delete(int id);
        Task Update(int id, UpdateResteurantDto dto);
    }
}
