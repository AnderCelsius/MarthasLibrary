using System.Net;

namespace MarthasLibrary.API.ErrorHandling;

public static class ErrorHandlingExtensions
{
  public static IApplicationBuilder UseErrorHandling(
    this IApplicationBuilder app,
    bool includeStackTrace,
    Dictionary<Type, HttpStatusCode>? errorMapping)

    => app.UseExceptionHandler(builder => builder.UseMiddleware<ExceptionHandler>(includeStackTrace, errorMapping));
}