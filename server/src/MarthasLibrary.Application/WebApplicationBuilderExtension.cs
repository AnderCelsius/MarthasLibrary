using MarthasLibrary.Application.UserData;
using Microsoft.AspNetCore.Builder;

namespace MarthasLibrary.Application;

public static class WebApplicationBuilderExtension
{
    public static void AddApplicationDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddUserDataProvider();
    }
}