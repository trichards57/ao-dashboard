// -----------------------------------------------------------------------
// <copyright file="PersistingAuthenticationStateProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AODashboard.Components.Account;

/// <summary>
/// Validation state provider that revalidates the current user every 30 minutes while connected.  It also passes the state
/// to the client.
/// </summary>
internal sealed class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly IdentityOptions options;
    private readonly PersistentComponentState state;
    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistingAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="persistentComponentState">The persistent state to pass to the client.</param>
    /// <param name="optionsAccessor">The Identity options.</param>
    public PersistingAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
    {
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="PersistingAuthenticationStateProvider"/> class.
    /// </summary>
    ~PersistingAuthenticationStateProvider()
    {
        // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Deal with any managed disposals.
            }

            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;

            disposedValue = true;
        }
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
            var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;

            if (userId != null && email != null)
            {
                state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Email = email,
                    OtherClaims = principal.Identities.Where(i => i.AuthenticationType == "Local").SelectMany(i => i.Claims),
                });
            }
        }
    }
}
