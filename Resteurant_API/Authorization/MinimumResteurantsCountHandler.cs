using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Resteurant_API.DataContext;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resteurant_API.Authorization
{
    public class MinimumResteurantsCountHandler : AuthorizationHandler<MinimumResteurantsCount>
    {
        private readonly ResteurantDbContext _dbContext;
        private readonly ILogger<MinimumResteurantsCountHandler> _logger;
        public MinimumResteurantsCountHandler(ResteurantDbContext resteurantDbContext, ILogger<MinimumResteurantsCountHandler> logger)
        {
            _dbContext = resteurantDbContext;
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumResteurantsCount requirement)
        {
            var userId = int.Parse(context.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var resteurantsCount = _dbContext.Resteurants.Count(r => r.CreatedById == userId);
            _logger.LogInformation($"User of id: {userId}");

            if (resteurantsCount >= requirement.MinCount)
            {
                context.Succeed(requirement);
                _logger.LogInformation("Authorization Succeeded");
            }
            else
            {
                context.Fail();
                _logger.LogInformation("Authorization Failed");
            }

            return Task.CompletedTask;
        }
    }
}
