// -----------------------------------------------------------------------
// <copyright file="ApplicationUser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;

namespace Dashboard.Data;

/// <summary>
/// Represents a user in the application.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the user's real name.
    /// </summary>
    [PersonalData]
    public string RealName { get; set; } = "";
}
