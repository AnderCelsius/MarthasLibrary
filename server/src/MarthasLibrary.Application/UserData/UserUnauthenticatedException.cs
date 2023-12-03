namespace MarthasLibrary.Application.UserData;

public class UserUnauthenticatedException : Exception
{
  public UserUnauthenticatedException()
    : base("The current user is not authenticated.")
  {
  }
}
