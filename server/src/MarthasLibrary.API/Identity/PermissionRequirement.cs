using Microsoft.AspNetCore.Authorization;

namespace MarthasLibrary.API.Identity;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; private set; } = permission;
}