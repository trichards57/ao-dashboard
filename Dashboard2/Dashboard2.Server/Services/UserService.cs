// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client;
using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
    public async IAsyncEnumerable<UserWithRole> GetUsersWithRole()
    {
        foreach (var user in userManager.Users)
        {
            var role = (await userManager.GetRolesAsync(user))?.FirstOrDefault();
            var roleId = role == null ? null : (await roleManager.FindByNameAsync(role))?.Id;

            yield return new UserWithRole
            {
                Id = user.Id,
                Name = user.RealName,
                Role = role,
                RoleId = roleId,
            };
        }
    }

    /// <inheritdoc/>
    public async Task<UserWithRole?> GetUserWithRole(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        var role = (await userManager.GetRolesAsync(user))?.FirstOrDefault();
        var roleId = role == null ? null : (await roleManager.FindByNameAsync(role))?.Id;

        return new UserWithRole
        {
            Id = user.Id,
            Name = user.RealName,
            Role = role,
            RoleId = roleId,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> SetUserRole(string id, UserRoleUpdate role)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return false;
        }

        var r = await roleManager.FindByIdAsync(role.RoleId);

        if (r == null)
        {
            return false;
        }

        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        await userManager.AddToRoleAsync(user, r.Name!);

        return true;
    }
}
