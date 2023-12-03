namespace MarthasLibrary.Core.Entities;

public interface IAuditableBase
{
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }
}