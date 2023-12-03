using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MarthasLibrary.Common.Authorization;


public class AuthorizationHeaderHandler : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var httpContext = _httpContextAccessor.HttpContext;
    var accessToken = await httpContext.GetTokenAsync("access_token");

    if (!string.IsNullOrEmpty(accessToken))
    {
      request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    }

    return await base.SendAsync(request, cancellationToken);
  }
}

