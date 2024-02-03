// -----------------------------------------------------------------------
// <copyright file="Places.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Immutable;

namespace AODashboard.Client.Model;

/// <summary>
/// Represents a list of places.
/// </summary>
public readonly record struct Places
{
    /// <summary>
    /// Gets the list of place names.
    /// </summary>
    public IImmutableList<string> Names { get; init; }
}
