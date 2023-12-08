using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarthasLibrary.Application.UserData;

public class UserDataProvider(
    IGenericRepository<Customer> customerRepository,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserDataProvider> logger)
  : IUserDataProvider<UserData>
{
  private readonly IGenericRepository<Customer> _customerRepository =
    customerRepository ?? throw new ArgumentException(nameof(customerRepository));

  private readonly IHttpContextAccessor _httpContextAccessor =
    httpContextAccessor ?? throw new ArgumentException(nameof(httpContextAccessor));

  private UserData? _currentUserData;

  public async Task<UserData?> GetCurrentUserData(CancellationToken cancellationToken = default)
  {
    if (_currentUserData is not null)
    {
      return _currentUserData;
    }

    try
    {
      var identityUserId = _httpContextAccessor.HttpContext.GetIdentityUserIdFromHttpContext();

      if (string.IsNullOrEmpty(identityUserId))
      {
        return null;
      }
      var customerData = await _customerRepository.Table.Where(customer => customer.IdentityUserId == identityUserId).SingleOrDefaultAsync(cancellationToken);
      if (customerData != null)
        _currentUserData = new UserData(customerData.Id, "", customerData.FirstName, customerData.LastName,
          customerData.Email);
    }
    catch (HttpRequestException ex)
    {
      logger.LogError(ex, "Error while retrieving customer data.");
      _currentUserData = null;
    }

    return _currentUserData;
  }
}