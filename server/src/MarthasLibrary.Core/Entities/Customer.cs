namespace MarthasLibrary.Core.Entities;

public class Customer : IAuditableBase
{
    public Customer(string firstName, string lastName, string email, string identityUserId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityUserId = identityUserId;
        Addresses = new HashSet<Address>();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string IdentityUserId { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<Address> Addresses { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void SetAsActive()
    {
        IsActive = true;
    }
}