// -----------------------------------------------------------------------
// <copyright file="PersistingRevalidatingAuthenticationStateProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Data;
using Dashboard.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace Dashboard.Components.Account;

/// <summary>
/// A server-side authentication state provider that revalidates the security stamp for the connected user
/// every 30 minutes an interactive circuit is connected. It also uses <see cref="PersistentComponentState"/> to flow the
/// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
/// </summary>
internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IdentityOptions options;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly PersistentComponentState state;
    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistingRevalidatingAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to use.</param>
    /// <param name="serviceScopeFactory">The service scope factory to use.</param>
    /// <param name="persistentComponentState">The persistent component state storage.</param>
    /// <param name="optionsAccessor">The options for this provider.</param>
    public PersistingRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        scopeFactory = serviceScopeFactory;
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    /// <inheritdoc/>
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task) => authenticationStateTask = task;

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
            var role = principal.FindFirst(options.ClaimsIdentity.RoleClaimType)?.Value;
            var amrUsed = principal.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            var lastAuthenticated = principal.FindFirst("auth_time")?.Value;

            if (userId != null && email != null)
            {
                state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Email = email,
                    Role = role ?? "None",
                    RealName = name ?? email,
                    AmrUsed = amrUsed ?? "Unknown",
                    LastAuthenticated = lastAuthenticated != null ? DateTimeOffset.Parse(lastAuthenticated, CultureInfo.InvariantCulture) : null,
                    OtherClaims = principal.Claims
                        .Where(c =>
                            c.Type != options.ClaimsIdentity.UserIdClaimType &&
                            c.Type != ClaimTypes.Name &&
                            c.Type != options.ClaimsIdentity.EmailClaimType &&
                            c.Type != options.ClaimsIdentity.RoleClaimType &&
                            c.Type != ClaimTypes.AuthenticationMethod &&
                            c.Type != "auth_time")
                        .ToDictionary(c => c.Type, c => c.Value),
                });
            }
        }
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }
}