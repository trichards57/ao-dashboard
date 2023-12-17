// -----------------------------------------------------------------------
// <copyright file="EasyAuthStateProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Client.Auth;

internal readonly record struct AuthClaim
{
    /// <summary>
    /// Gets the claim's type.
    /// </summary>
    [JsonPropertyName("typ")]
    public string Type { get; init; }

    /// <summary>
    /// Gets the claim's value.
    /// </summary>
    [JsonPropertyName("val")]
    public string Value { get; init; }
}

internal readonly record struct ClientPrincipal
{
    /// <summary>
    /// Gets the identity provider used to log in.
    /// </summary>
    [JsonPropertyName("identityProvider")]
    public string IdentityProvider { get; init; }

    /// <summary>
    /// Gets the user's ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; init; }

    /// <summary>
    /// Gets the user's details.
    /// </summary>
    [JsonPropertyName("userDetails")]
    public string UserDetails { get; init; }

    /// <summary>
    /// Gets the user's roles.
    /// </summary>
    [JsonPropertyName("userRoles")]
    public IEnumerable<string> UserRoles { get; init; }

    /// <summary>
    /// Gets the user's claims.
    /// </summary>
    [JsonPropertyName("claims")]
    public IEnumerable<AuthClaim> Claims { get; init; }
}

internal readonly record struct AuthData
{
    /// <summary>
    /// Gets the information about the current user.
    /// </summary>
    [JsonPropertyName("clientPrincipal")]
    public ClientPrincipal ClientPrincipal { get; init; }
}

/// <summary>
/// Authentication State Provider that gives the current EasyAuth state.
/// </summary>
/// <param name="client">The <see cref="HttpClient" /> to use for requesting the current state.</param>
public class EasyAuthStateProvider(HttpClient client) : AuthenticationStateProvider
{
    /// <inheritdoc/>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var data = await client.GetFromJsonAsync<AuthData>("/.auth/me");

            var identity = new ClaimsIdentity(data.ClientPrincipal.Claims.Select(c => new Claim(c.Type, c.Value)), data.ClientPrincipal.IdentityProvider);
            identity.AddClaim(new Claim(ClaimTypes.Name, data.ClientPrincipal.UserDetails));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, data.ClientPrincipal.UserId));
            identity.AddClaim(new Claim(ClaimTypes.Email, data.ClientPrincipal.UserId));

            foreach (var i in data.ClientPrincipal.UserRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, i));
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity(identity));

            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Did not log in. {ex.Message}.");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
