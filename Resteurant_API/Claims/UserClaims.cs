using Resteurant_API.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace Resteurant_API.Claims
{
    public static class UserClaims
    {
        private static List<Claim> Claims;

        public static List<Claim> CreateClaims(User user)
        {
            Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
            };

            if (!string.IsNullOrEmpty(user.Nationality))
                Claims.Add(new Claim("Nationality", user.Nationality));

            return Claims;
        }
    }
}
