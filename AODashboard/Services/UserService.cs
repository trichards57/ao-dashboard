// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.Security.Claims;

namespace AODashboard.Services;

/// <summary>
/// Represents a service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the claims for the provided user.
    /// </summary>
    /// <param name="userId">The ID of the user to check for.</param>
    /// <returns>The extra claims to add to the user.</returns>
    IAsyncEnumerable<Claim> GetClaimsAsync(string userId);

    /// <summary>
    /// Gets the current user's photograph.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    ///
    /// Resolves to a memory stream containing the photo as a JPEG, or null
    /// if none is available.
    /// </returns>
    Task<MemoryStream?> GetProfilePictureAsync();
}

/// <summary>
/// Constants representing the user claims.
/// </summary>
internal static class UserClaims
{
    /// <summary>
    /// Gets the value when the user has permission to read or edit.
    /// </summary>
    public static string Edit => "Edit";

    /// <summary>
    /// Gets the claim for the user's permissions permission level.
    /// </summary>
    public static string Permissions => "Permissions";

    /// <summary>
    /// Gets the value when the user has permission to read.
    /// </summary>
    public static string Read => "Read";

    /// <summary>
    /// Gets the claim for the user's sensitive permissions permission level.
    /// </summary>
    public static string SensitivePermissions => "SensitivePermissions";

    /// <summary>
    /// Gets the claim for the user's vehicle configuration permission level.
    /// </summary>
    public static string VehicleConfiguration => "VehicleConfiguration";

    /// <summary>
    /// Gets the claim for the user's VOR Data permission level.
    /// </summary>
    public static string VorData => "VORData";
}

/// <summary>
/// Service to manage users.
/// </summary>
/// <param name="context">The data context to store data in.</param>
/// <param name="graphServiceClient">Client to access the Graph API.</param>
internal class UserService(ApplicationDbContext context, GraphServiceClient graphServiceClient) : IUserService
{
    /// <inheritdoc/>
    public async Task<MemoryStream?> GetProfilePictureAsync()
    {
        await graphServiceClient.Me.GetAsync();

        var result = await graphServiceClient.Me.Photos["48x48"].Content.GetAsync();

        if (result == null)
        {
            return null;
        }

        var newStream = new MemoryStream();
        result.CopyTo(newStream);

        newStream.Position = 0;

        return newStream;
    }

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