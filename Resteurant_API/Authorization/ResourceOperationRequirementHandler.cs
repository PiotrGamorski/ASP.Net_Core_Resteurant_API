using Microsoft.AspNetCore.Authorization;
using Resteurant_API.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resteurant_API.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Resteurant>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Resteurant resteurant)
        {
            if (requirement.ResourceOperation == ResourceOperation.Create ||
                requirement.ResourceOperation == ResourceOperation.Read)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            if (int.Parse(userId) == resteurant.CreatedById)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
