namespace MarthasLibrary.API.Middleware;

public class AddUserNameToHttpContextMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            context.Items["UserName"] = context.User.Identity.Name;
        }

        await next(context);
    }
}
