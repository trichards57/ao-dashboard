// -----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

/// <summary>
/// Represents the permission levels for a role's permissions.
/// </summary>
public enum ReadWrite
{
    /// <summary>
    /// The role may read this data.
    /// </summary>
    Read,

    /// <summary>
    /// The role may read and update this data.
    /// </summary>
    Write,

    /// <summary>
    /// The role may not see this data.
    /// </summary>
    Deny,
}
