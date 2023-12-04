namespace MarthasLibrary.Application.UserData;

public interface IUserDataProvider<T>
  where T : UserBasicData
{
  /// <summary>
  /// Retrieves current user's data.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>Current user's data. Null if data could not be found.</returns>
  Task<T?> GetCurrentUserData(CancellationToken cancellationToken = default);
}
