using Microsoft.AspNetCore.Authorization;

namespace HomeBudget.Authorization
{
    public enum ResourceOperation
    {
        Create,
        Update,
        Delete,
        Read
    }
    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(ResourceOperation resourceOperation)
        {
            ResourceOperation = resourceOperation;
        }
        
        public ResourceOperation ResourceOperation { get; }
    }
}
