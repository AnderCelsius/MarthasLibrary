namespace MarthasLibrary.Core.Entities;

public class Address : IAuditableBase
{
  private Address(string customerId, string street, string city, string state, string country, string zipCode)
  {
    CustomerId = customerId;
    Street = street;
    City = city;
    State = state;
    Country = country;
    ZipCode = zipCode;
    CreatedAt = DateTime.Now;
    UpdatedAt = DateTime.Now;
  }

  public Guid Id { get; private set; }
  public string CustomerId { get; private set; }
  public string Street { get; private set; }
  public string City { get; private set; }
  public string State { get; private set; }
  public string Country { get; private set; }
  public string ZipCode { get; private set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }

  public static Address CreateInstance(string customerId, string street, string city, string state, string country, string zipCode)
  {
    return new Address(customerId, street, city, state, country, zipCode);
  }
}