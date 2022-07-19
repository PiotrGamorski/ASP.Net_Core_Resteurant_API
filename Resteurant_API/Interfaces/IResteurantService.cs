using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resteurant_API.Interfaces
{
    public interface IResteurantService
    {
        Task<ResteurantDto> GetById(int id);
        Task<IEnumerable<ResteurantDto>> GetAll();
        Task<Resteurant> Create(CreateResteurantDto dto);
        Task Delete(int id);
        Task Update(int id, UpdateResteurantDto dto);
    }
}
