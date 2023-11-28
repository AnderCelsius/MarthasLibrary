using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MarthasLibrary.API.ErrorHandling;

public class ExceptionHandler
{
  private readonly RequestDelegate next;
  private readonly bool includeStackTrace;
  private readonly Dictionary<Type, HttpStatusCode>? errorMapping;

  public ExceptionHandler(
    RequestDelegate next,
    bool includeStackTrace,
    Dictionary<Type, HttpStatusCode>? errorMapping = null)
  {
    this.next = next;
    this.includeStackTrace = includeStackTrace;
    this.errorMapping = errorMapping;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var error = exceptionHandlerPathFeature?.Error;

    if (error is not null)
    {
      var cancellationToken = context.RequestAborted;

      var errorType = error.GetType();

      var problemDetails = new ProblemDetails
      {
        Instance = context.Request.Path,
        Title = errorType.Name,
      };

      if (errorMapping?.TryGetValue(errorType, out var httpStatusCode) == true)
      {
        problemDetails.Status = (int)httpStatusCode;
        problemDetails.Detail = error.Message;
      }
      else if (error is UnauthorizedException)
      {
        problemDetails.Status = StatusCodes.Status401Unauthorized;
        problemDetails.Detail = error.Message;
      }
      else if (error is NotFoundException)
      {
        problemDetails.Status = StatusCodes.Status404NotFound;
        problemDetails.Detail = error.Message;
      }
      else if (error is InvalidOperationException or ArgumentNullException)
      {
        problemDetails.Status = StatusCodes.Status400BadRequest;
        problemDetails.Detail = error.Message;
      }
      else if (error is ForbiddenException)
      {
        problemDetails.Status = StatusCodes.Status403Forbidden;
        problemDetails.Detail = error.Message;
      }
      else
      {
        problemDetails.Status = StatusCodes.Status500InternalServerError;

        if (includeStackTrace)
        {
          problemDetails.Detail = error.ToString();
        }
      }

      context.Response.StatusCode = problemDetails.Status.Value;
      await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
  }
}
