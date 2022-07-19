using Microsoft.AspNetCore.Authorization;

namespace Resteurant_API.Authorization
{
    public class MinimumResteurantsCount : IAuthorizationRequirement
    {
        public int MinCount { get; }
        public MinimumResteurantsCount(int minCount)
        {
            MinCount = minCount;
        }
    }
}
