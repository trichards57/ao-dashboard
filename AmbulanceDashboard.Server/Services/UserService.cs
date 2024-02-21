// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Data;
using AmbulanceDashboard.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AmbulanceDashboard.Services;

/// <summary>
/// Service to manage users.
/// </summary>
/// <param name="context">The data context to store data in.</param>
internal sealed class UserService(ApplicationDbContext context) : IUserService
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<Claim> GetClaimsAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var roles = await context.UserRoles.Where(u => u.UserId == userId && !u.Role!.Name.Contains("Administrator")).Select(s => s.Role).Cast<Role>().ToListAsync();

        if (roles.Count == 0)
        {
            yield break;
        }

        List<Claim?> claims = [GetClaim(UserClaims.VehicleConfiguration, roles.Max(r => r.VehicleConfiguration)),
            GetClaim(UserClaims.Permissions, roles.Max(r => r.Permissions)),
            GetClaim(UserClaims.SensitivePermissions, roles.Max(r => r.SensitivePermissions)),
            GetClaim(UserClaims.VorData, roles.Max(r => r.VorData))];

        foreach (var c in claims)
        {
            if (c != null)
            {
                yield return c;
            }
        }
    }

    private static Claim? GetClaim(string claimId, PermissionLevel level)
    {
        if (level >= PermissionLevel.ReadWrite)
        {
            return new Claim(claimId, UserClaims.Edit);
        }
        else if (level >= PermissionLevel.Read)
        {
            return new Claim(claimId, UserClaims.Read);
        }

        return null;
    }
}
