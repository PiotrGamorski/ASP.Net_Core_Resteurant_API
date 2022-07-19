using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Resteurant_API.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resteurant_API.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private ILogger<MinimumAgeRequirementHandler> _logger;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(claim => claim.Type == "DateOfBirth").Value);
            var userEmail = context.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            _logger.LogInformation($"User: {userEmail} with date of birth: [{dateOfBirth}]");

            if (DateTime.Today.Year - dateOfBirth.Year >= requirement.MinAge)
            {
                _logger.LogInformation("Authorization Succedded");
                context.Succeed(requirement);
            }
            else _logger.LogInformation("Authorization Failed");

            return Task.CompletedTask;
        }
    }
}
