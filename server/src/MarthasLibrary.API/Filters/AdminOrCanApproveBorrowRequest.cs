namespace MarthasLibrary.API.Filters;

//public class AdminOrCanApproveBorrowRequestAuthorizationHandler : AuthorizationHandler<AdminOrCanApproveBorrowRequestRequirement>
//{
//    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrCanApproveBorrowRequestRequirement requirement)
//    {
//        if (context.User.IsInRole("admin"))
//        {
//            context.Succeed(requirement);
//        }
//        else if (context.User.HasClaim(c => c.Type == "CanApproveBorrowRequest" && c.Value == "true"))
//        {
//            context.Succeed(requirement);
//        }

//        return Task.CompletedTask;
//    }
//}
