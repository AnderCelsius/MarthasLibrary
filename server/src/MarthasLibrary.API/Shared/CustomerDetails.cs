namespace MarthasLibrary.API.Shared;

public record CustomerDetails(
  Guid Id,
  string Email,
  string FirstName,
  string LastName,
  CustomerDetails.AddressDetails? PrimaryAddress,
  string? PhoneNumber,
  DateTimeOffset CreatedAt
)
{
  public record AddressDetails(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode);
};