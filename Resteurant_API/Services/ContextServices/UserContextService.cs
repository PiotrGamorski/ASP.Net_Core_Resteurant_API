using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Resteurant_API.Services.ContextServices
{
    public interface IUserContextService
    {
        int? GetUserId { get; }
        ClaimsPrincipal User { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
        public int? GetUserId =>
            User is null ? null : int.Parse(User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
    }
}
