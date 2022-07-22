using Microsoft.AspNetCore.Authorization;

namespace Resteurant_API.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinAge { get; }
        public MinimumAgeRequirement(int minAge)
        {
            MinAge = minAge;
        }
    }
}
