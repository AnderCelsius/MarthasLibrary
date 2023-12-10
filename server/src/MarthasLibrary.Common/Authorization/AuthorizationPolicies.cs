using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MarthasLibrary.Common.Authorization
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy CanAddBook()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Policies.LibraryStaff)
                .Build();
        }

        public static AuthorizationPolicy CanAddLibraryStaff()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Policies.IsAdmin)
                .Build();
        }

        public static AuthorizationPolicy CanApproveBorrowRequest()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new RolesAuthorizationRequirement(new[] { "Admin" }),
                    new ClaimsAuthorizationRequirement("CanApproveBorrowRequest", new List<string> { "true" })
                )
                .Build();
        }

    }
}