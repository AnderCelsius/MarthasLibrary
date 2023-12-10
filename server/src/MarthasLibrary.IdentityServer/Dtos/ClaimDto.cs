#nullable enable
namespace MarthasLibrary.IdentityServer.Dtos
{
    public record ClaimDto(string Type, string? Value, bool Selected);
}
