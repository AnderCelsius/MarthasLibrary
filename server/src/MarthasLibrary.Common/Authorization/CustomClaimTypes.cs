namespace MarthasLibrary.Common.Authorization;

public static class CustomClaimTypes
{
  public const string Scope = "scope";
  public const string FirstName = "FirstName";
  public const string LastName = "LastName";
  public const string AdminOrCanApproveBorrowRequest = nameof(AdminOrCanApproveBorrowRequest);
}
