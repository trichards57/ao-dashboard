// -----------------------------------------------------------------------
// <copyright file="PersistentAuthenticationStateProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Dashboard.Client;

/// <summary>
/// A persistent authentication state provider that uses data persisted in the page to determine the user's
/// authentication state. This is fixed for the lifetime of the WebAssembly application. A full page reload
/// will be required to log in or out. This only provides a user name and email for display purposes. It does
/// actually include any tokens that authenticate to the server when making subsequent requests. That works
/// using a cookie that will be included on HttpClient requests to the server.
/// </summary>
internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = DefaultUnauthenticatedTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="state">The persistent component state store.</param>
    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }

        Claim[] claims = [.. userInfo.OtherClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)),
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.RealName),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.Role, userInfo.Role),
            new Claim(ClaimTypes.AuthenticationMethod, userInfo.AmrUsed),
            new Claim("auth_time", userInfo.LastAuthenticated?.ToString("o") ?? "")
        ];

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
                claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    /// <inheritdoc/>
    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
