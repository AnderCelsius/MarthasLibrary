namespace MarthasLibrary.Application.UserData;

public record UserData(
  Guid Id,
  string Type,
  string FirstName,
  string LastName,
  string Email
 ) : UserBasicData(Email, Type);
