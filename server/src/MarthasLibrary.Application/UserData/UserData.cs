namespace MarthasLibrary.Application.UserData;

public record UserData(
  string Type,
  string FirstName,
  string LastName,
  string Email,
  string Id,
  List<UserData.AddressSet> Addresses,
  List<string> Restrictions) : UserBasicData(Email, Type)
{
  public record AddressSet(
    string Name,
    List<string> Addresses);
}
