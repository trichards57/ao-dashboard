// -----------------------------------------------------------------------
// <copyright file="IdentityRedirectManager.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Dashboard.Components.Account;

/// <summary>
/// Redirect manager for Identity components.
/// </summary>
internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    /// <summary>
    /// The name of the cookie that stores the status message.
    /// </summary>
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    /// <summary>
    /// Redirects to the specified URI with the specified query parameters.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    /// <param name="queryParameters">The query parameters to add.</param>
    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }

    /// <summary>
    /// Redirects to the current page.
    /// </summary>
    [DoesNotReturn]
    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    /// <summary>
    /// Redirects to the current page with the specified query parameters.
    /// </summary>
    /// <param name="queryParameters">The query parameters to add.</param>
    [DoesNotReturn]
    public void RedirectToCurrentPage(Dictionary<string, object?> queryParameters) => RedirectTo(CurrentPath, queryParameters);

    /// <summary>
    /// Redirects to the current page with the specified status message.
    /// </summary>
    /// <param name="message">The status message.</param>
    /// <param name="context">The HTTP Context to use.</param>
    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        => RedirectToWithStatus(CurrentPath, message, context);

    /// <summary>
    /// Redirects to the specified URI with the specified status message.
    /// </summary>
    /// <param name="uri">The URI to redirect to.</param>
    /// <param name="message">The status message.</param>
    /// <param name="context">The HTTP Context to use.</param>
    [DoesNotReturn]
    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        RedirectTo(uri);
    }
}