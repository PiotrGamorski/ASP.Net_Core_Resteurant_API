using Resteurant_API.Dtos;
using System.Threading.Tasks;

namespace Resteurant_API.Interfaces
{
    public interface IAccountService
    {
        Task RegisterUser(RegisterUserDto dto);
        Task<string> Login_UseJSONWebToken(LoginUserDto dto);
        Task Login_UseCookies(LoginUserDto dto);
        Task Logout();
    }
}
