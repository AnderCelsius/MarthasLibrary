namespace MarthasLibrary.Common.Authorization;

public static class Policies
{
    public const string IsAdmin = "Admin";
    public const string CanApproveBorrowRequest = nameof(CanApproveBorrowRequest);
    public const string ClientApplicationCanWrite = nameof(ClientApplicationCanWrite);
    public const string LibraryStaff = nameof(LibraryStaff);
}
