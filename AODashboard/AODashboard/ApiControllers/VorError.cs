// -----------------------------------------------------------------------
// <copyright file="VorError.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.ApiControllers;

/// <summary>
/// An error related to a VOR entry update.
/// </summary>
public readonly record struct VorError
{
    /// <summary>
    /// Gets the error reported.
    /// </summary>
    public string Error { get; init; }
}
