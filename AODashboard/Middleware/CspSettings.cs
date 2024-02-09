// -----------------------------------------------------------------------
// <copyright file="CspSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Middleware;

/// <summary>
/// Represents the settings for the Content Security Policy (CSP) middleware.
/// </summary>
public class CspSettings
{
    /// <summary>
    /// Gets the permitted style sources.
    /// </summary>
    public IEnumerable<string> StyleSource { get; init; } = [];

    /// <summary>
    /// Gets the permitted font sources.
    /// </summary>
    public IEnumerable<string> FontSource { get; init; } = [];

    /// <summary>
    /// Gets the permitted connection sources.
    /// </summary>
    public IEnumerable<string> ConnectSource { get; init; } = [];
}
