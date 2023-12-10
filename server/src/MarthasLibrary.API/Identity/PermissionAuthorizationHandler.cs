using MarthasLibrary.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace MarthasLibrary.API.Identity;

public class PermissionAuthorizationHandler(LibraryDbContext dbContext) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User == null)
        {
            return;
        }

        var permissions = context.User.Claims.Where(x => x.Type == "Permission" &&
                                x.Value == requirement.Permission);

        if (permissions.Any())
        {
            context.Succeed(requirement);
        }
    }
}
