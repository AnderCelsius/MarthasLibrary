namespace MarthasLibrary.Common.Authorization;

public static class Policies
{
    public const string IsAdmin = nameof(IsAdmin);
    public const string CanApproveBorrowRequest = nameof(CanApproveBorrowRequest);
    public const string ClientApplicationCanWrite = nameof(ClientApplicationCanWrite);
    public const string UserCanAddBook = nameof(UserCanAddBook);
}
