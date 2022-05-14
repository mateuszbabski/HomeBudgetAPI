using HomeBudget.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HomeBudget.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Budget>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Budget budget)
        {
            
            var userId =  context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if(budget.UserID == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
