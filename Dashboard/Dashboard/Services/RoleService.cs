using Dashboard.Client;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Dashboard.Services;

internal class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    private readonly RoleManager<IdentityRole> roleManager = roleManager;
   
    public async Task<RolePermissions?> GetRolePermissions(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return null;
        }

        var claims = await roleManager.GetClaimsAsync(role);

        return new RolePermissions
        {
            Name = role.Name ?? "",
            VehicleConfiguration = ClaimToReadWrite(claims, UserClaims.VehicleConfiguration),
            VorData = ClaimToReadWrite(claims, UserClaims.VorData),
            Permissions = ClaimToReadWrite(claims, UserClaims.Permissions),
        };
    }

    public async Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return false;
        }

        var claims = await roleManager.GetClaimsAsync(role);

        await UpdateClaim(role, claims, UserClaims.VehicleConfiguration, permissions.VehicleConfiguration);
        await UpdateClaim(role, claims, UserClaims.VorData, permissions.VorData);
        await UpdateClaim(role, claims, UserClaims.Permissions, permissions.Permissions);

        return true;
    }

    private async Task UpdateClaim(IdentityRole role, IEnumerable<Claim> claims, string type, ReadWrite value)
    {
        if (ClaimToReadWrite(claims, type) == value)
        {
            return;
        }

        var old = claims.FirstOrDefault(c => c.Type == type);

        if (old != null)
        {
            await roleManager.RemoveClaimAsync(role, old);
        }

        if (value == ReadWrite.Read)
        {
            await roleManager.AddClaimAsync(role, new Claim(type, UserClaims.Read));
        }
        else if (value == ReadWrite.Write)
        {
            await roleManager.AddClaimAsync(role, new Claim(type, UserClaims.Edit));
        }
    }

    private static ReadWrite ClaimToReadWrite(IEnumerable<Claim> claims, string type)
    {
        var c = claims.FirstOrDefault(c => c.Type == type);

        if (c?.Value == UserClaims.Read)
        {
            return ReadWrite.Read;
        }
        else if (c?.Value == UserClaims.Edit)
        {
            return ReadWrite.Write;
        }

        return ReadWrite.Deny;
    }
}
