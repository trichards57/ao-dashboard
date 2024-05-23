// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.Services;

/// <summary>
/// Server-side service for managing roles.
/// </summary>
/// <param name="roleManager">The role manager to use.</param>
internal class UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly RoleManager<IdentityRole> roleManager = roleManager;
    private readonly UserManager<ApplicationUser> userManager = userManager;

    /// <inheritdoc/>
    public async IAsyncEnumerable<Grpc.UserWithRole> GetUsersWithRole()
    {
        foreach (var user in userManager.Users)
        {
            yield return await ToUserWithRole(user);
        }
    }

    /// <inheritdoc/>
    public async Task<Grpc.UserWithRole?> GetUserWithRole(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        return await ToUserWithRole(user);
    }

    private async Task<Grpc.UserWithRole> ToUserWithRole(ApplicationUser user)
    {
        var role = (await userManager.GetRolesAsync(user))?.FirstOrDefault() ?? string.Empty;
        var roleId = string.IsNullOrEmpty(role) ? string.Empty : ((await roleManager.FindByNameAsync(role))?.Id ?? string.Empty);

        return new Grpc.UserWithRole
        {
            Id = user.Id,
            Name = user.RealName,
            Role = role,
            RoleId = roleId,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> SetUserRole(Grpc.UpdateUserRequest update)
    {
        var user = await userManager.FindByIdAsync(update.Id);

        if (user == null)
        {
            return false;
        }

        var r = await roleManager.FindByIdAsync(update.RoleId);

        if (r == null)
        {
            return false;
        }

        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        await userManager.AddToRoleAsync(user, r.Name!);

        return true;
    }
}
