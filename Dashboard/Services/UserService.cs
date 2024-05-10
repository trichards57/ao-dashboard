// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Services;

/// <summary>
/// Server-side service for managing roles.
/// </summary>
/// <param name="roleManager">The role manager to use.</param>
internal class UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly RoleManager<IdentityRole> roleManager = roleManager;
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly ApplicationDbContext context = context;

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

    /// <inheritdoc/>
    public async Task<bool> InviteUser(UserInviteRequest request, string invitingUserId)
    {
        var invite = new UserInvite
        {
            CreatedById = invitingUserId,
            Email = request.Email,
            RoleId = request.RoleId,
        };

        try
        {
            context.Add(invite);
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task CancelInvite(string email)
    {
        var invite = await context.UserInvites.FirstOrDefaultAsync(i => i.Email == email);

        if (invite != null)
        {
            context.Remove(invite);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<UserInviteSummary> GetAllInvites() => context.UserInvites.Select(s => new UserInviteSummary
    {
        Id = s.Id,
        Email = s.Email,
        Redeemed = s.Accepted.HasValue,
        Created = s.Created,
        CreatedBy = s.CreatedBy.RealName,
    }).AsAsyncEnumerable();
}
