// -----------------------------------------------------------------------
// <copyright file="ConnectApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client;
using Dashboard2.Server.Model.OpenId;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Dashboard2.Server.Api;

internal static class ConnectApiExtensions
{
    internal static WebApplication MapConnect(this WebApplication app)
    {
        var group = app.MapGroup("/connect");

        group.MapPost("token", async (HttpContext context, [FromServices] IOpenIddictApplicationManager appManager, [FromServices] IOpenIddictScopeManager scopeManager) =>
        {
            var request = context.GetOpenIddictServerRequest()!;

            if (request.IsClientCredentialsGrantType())
            {
                var application = await appManager.FindByClientIdAsync(request.ClientId!) ?? throw new InvalidOperationException("The application details cannot be found in the database.");

                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
                identity.SetClaim(Claims.Subject, await appManager.GetClientIdAsync(application));
                identity.SetClaim(Claims.Name, await appManager.GetDisplayNameAsync(application));

                if (await appManager.HasPermissionAsync(application, "vor:edit"))
                {
                    identity.AddClaim(UserClaims.VorData, UserClaims.Edit);
                }

                identity.SetScopes(request.GetScopes());
                identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
                identity.SetDestinations(GetDestinations);
                await context.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            throw new InvalidOperationException("The specified grant is invalid.");
        });

        return app;
    }

    private static IEnumerable<string> GetDestinations(Claim claim) => claim.Type switch
    {
        Claims.Name or Claims.Subject => [Destinations.AccessToken, Destinations.IdentityToken],
        _ => [Destinations.AccessToken],
    };
}
