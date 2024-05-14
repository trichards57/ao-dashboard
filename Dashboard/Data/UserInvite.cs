// -----------------------------------------------------------------------
// <copyright file="UserInvite.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dashboard.Data;

/// <summary>
/// Represents an invitation for a user to join the website.
/// </summary>
public class UserInvite
{
    /// <summary>
    /// Gets or sets the ID of the invite.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [Required]
    [MaxLength(254)]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the ID of the assigned role for the user.
    /// </summary>
    public required string RoleId { get; set; }

    /// <summary>
    /// Gets or sets the assigned role for the user.
    /// </summary>
    [NotNull]
    public IdentityRole? Role { get; set; }

    /// <summary>
    /// Gets or sets the date the invite was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user that created the invite.
    /// </summary>
    public required string CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the ID the user that created the invite.
    /// </summary>
    [NotNull]
    public ApplicationUser? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date the invite was accepted.
    /// </summary>
    public DateTimeOffset? Accepted { get; set; }
}
