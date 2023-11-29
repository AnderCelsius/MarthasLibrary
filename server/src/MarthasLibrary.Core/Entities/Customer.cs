using Microsoft.AspNetCore.Identity;

namespace MarthasLibrary.Core.Entities;

public class Customer : IdentityUser, IAuditableBase
{
    public Customer(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Addresses = new HashSet<Address>();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; set; }

    public ICollection<Address> Addresses { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}