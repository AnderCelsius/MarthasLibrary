using Microsoft.AspNetCore.Authorization;

namespace MarthasLibrary.Common.Authorization
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy CanAddBook()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Policies.IsAdmin)
                .Build();
        }
    }
}