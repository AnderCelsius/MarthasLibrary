namespace MarthasLibrary.Core.Entities;

public class Customer : IAuditableBase
{
  private Customer(string firstName, string lastName, string email, string identityUserId)
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

  public static Customer CreateInstance(string firstName, string lastName, string email, string identityUserId)
  {
    return new Customer(firstName, lastName, email, identityUserId);
  }

  public void SetAsActive()
  {
    IsActive = true;
  }

  /// <summary>
  /// Replaces the user's properties with those provided in the UserUpdate.
  /// </summary>
  /// <param name="update">Provides a <see cref="UserUpdate"/> object that contains the updated data for the user.</param>
  public void UpdateDetails(UserUpdate update)
  {
    FirstName = update.FirstName;
    LastName = update.LastName;
    UpdatedAt = DateTimeOffset.UtcNow;
  }

  public record UserUpdate(string FirstName, string LastName);
}