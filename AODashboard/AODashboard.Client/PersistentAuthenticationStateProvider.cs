// -----------------------------------------------------------------------
// <copyright file="PersistentAuthenticationStateProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AODashboard.Client;

/// <summary>
/// <para>
/// Client-side <see cref="AuthenticationStateProvider"/> that looks for authentication state in the data
/// provided by the server on render.  This is fixed for the lifetime of the application, so the application
/// needs to reload if the user logs in or out.
/// </para>
/// <para>
/// This only provides username and email for display.  It does not include any tokens, these are stored
/// separately in a cookie included in HttpClient requests to the server.
/// </para>
/// </summary>
internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = DefaultUnauthenticatedTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="state">The persistent state provided by the server.</param>
    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }

        Claim[] claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Email),
            new Claim(ClaimTypes.Email, userInfo.Email)];

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims.Concat(userInfo.OtherClaims), authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    /// <inheritdoc/>
    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
