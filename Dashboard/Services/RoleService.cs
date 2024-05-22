// -----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dashboard.Services;

/// <summary>
/// Server-side service for managing roles.
/// </summary>
/// <param name="roleManager">The role manager to use.</param>
internal class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    private readonly RoleManager<IdentityRole> roleManager = roleManager;

    /// <inheritdoc/>
    public async Task<Grpc.RolePermissions?> GetRolePermissions(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return null;
        }

        var claims = await roleManager.GetClaimsAsync(role);

        return new Grpc.RolePermissions
        {
            Id = role.Id,
            Name = role.Name ?? "",
            VehicleConfiguration = ClaimToReadWrite(claims, UserClaims.VehicleConfiguration),
            VorData = ClaimToReadWrite(claims, UserClaims.VorData),
            Permissions = ClaimToReadWrite(claims, UserClaims.Permissions),
        };
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Grpc.RolePermissions> GetRoles()
    {
        var roleList = roleManager.Roles.ToList();

        foreach (var r in roleList)
        {
            var claims = await roleManager.GetClaimsAsync(r);

            yield return new Grpc.RolePermissions
            {
                Id = r.Id,
                Name = r.Name ?? "",
                Permissions = CheckPermission(claims, UserClaims.Permissions),
                VehicleConfiguration = CheckPermission(claims, UserClaims.VehicleConfiguration),
                VorData = CheckPermission(claims, UserClaims.VorData),
            };
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetRolePermissions(Grpc.UpdateRoleRequest permissions)
    {
        var role = await roleManager.FindByIdAsync(permissions.Id);

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

    private static Grpc.ReadWrite CheckPermission(IList<Claim> claims, string type)
    {
        var c = claims.FirstOrDefault(k => k.Type == type);

        if (c == null)
        {
            return Grpc.ReadWrite.Deny;
        }
        else if (c.Value == UserClaims.Read)
        {
            return Grpc.ReadWrite.ReadOnly;
        }
        else if (c.Value == UserClaims.Edit)
        {
            return Grpc.ReadWrite.ReadWrite;
        }

        return Grpc.ReadWrite.Deny;
    }

    private static Grpc.ReadWrite ClaimToReadWrite(IEnumerable<Claim> claims, string type)
    {
        var c = claims.FirstOrDefault(c => c.Type == type);

        if (c?.Value == UserClaims.Read)
        {
            return Grpc.ReadWrite.ReadOnly;
        }
        else if (c?.Value == UserClaims.Edit)
        {
            return Grpc.ReadWrite.ReadWrite;
        }

        return Grpc.ReadWrite.Deny;
    }

    private async Task UpdateClaim(IdentityRole role, IEnumerable<Claim> claims, string type, Grpc.ReadWrite value)
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

        if (value == Grpc.ReadWrite.ReadOnly)
        {
            await roleManager.AddClaimAsync(role, new Claim(type, UserClaims.Read));
        }
        else if (value == Grpc.ReadWrite.ReadWrite)
        {
            await roleManager.AddClaimAsync(role, new Claim(type, UserClaims.Edit));
        }
    }
}
