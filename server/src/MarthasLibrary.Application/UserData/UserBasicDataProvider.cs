using Microsoft.AspNetCore.Http;

namespace MarthasLibrary.Application.UserData;

public class UserBasicDataProvider : IUserDataProvider<UserBasicData>
{
  private readonly IHttpContextAccessor httpContextAccessor;

  public UserBasicDataProvider(IHttpContextAccessor httpContextAccessor)
  {
    this.httpContextAccessor = httpContextAccessor;
  }

  public Task<UserBasicData?> GetCurrentUserData(CancellationToken cancellationToken = default)
  {
    var userEmail = httpContextAccessor.HttpContext?.GetEmailFromHttpContext();
    if (userEmail is null)
    {
      return Task.FromResult<UserBasicData?>(null);
    }

    var userScope = httpContextAccessor.HttpContext?.GetScopeFromHttpContext();
    if (userScope is null)
    {
      return Task.FromResult<UserBasicData?>(null);
    }

    return Task.FromResult<UserBasicData?>(new UserBasicData(userEmail, userScope));
  }
}
