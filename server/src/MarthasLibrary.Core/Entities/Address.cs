using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.Core.Entities;

public class Address : IAuditableBase
{
  private Address(Guid customerId, string street, string city, string state, string country, string zipCode)
  {
    CustomerId = customerId;
    Street = street;
    City = city;
    State = state;
    Country = country;
    ZipCode = zipCode;
    CreatedAt = DateTimeOffset.UtcNow;
    UpdatedAt = DateTimeOffset.UtcNow;
  }

  private Address(Guid customerId, string street, string city, string state, string country, string zipCode,
    AddressType addressType)
    : this(customerId, street, city, state, country, zipCode)
  {
    AddressType = addressType;
  }

  public Guid Id { get; private set; }
  public Guid CustomerId { get; private set; }
  public AddressType AddressType { get; private set; }
  public string Street { get; private set; }
  public string City { get; private set; }
  public string State { get; private set; }
  public string Country { get; private set; }
  public string ZipCode { get; private set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

  public static Address CreateInstance(Guid customerId, string street, string city, string state, string country,
    string zipCode)
  {
    return new Address(customerId, street, city, state, country, zipCode);
  }

  public static Address CreateInstance(Guid customerId, string street, string city, string state, string country,
    string zipCode, AddressType addressType)
  {
    return new Address(customerId, street, city, state, country, zipCode, addressType);
  }
}