namespace MarthasLibrary.Core.Entities;

public class Address : IAuditableBase
{
  private Address(Customer customer, string street, string city, string state, string country, string zipCode)
  {
    CustomerId = customer.Id;
    Street = street;
    City = city;
    State = state;
    Country = country;
    ZipCode = zipCode;
    Customer = customer;
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

  // Navigation property back to the Customer
  public Customer Customer { get; set; }

  public static Address CreateInstance(Customer customer, string street, string city, string state, string country, string zipCode)
  {
    return new Address(customer, street, city, state, country, zipCode);
  }
}