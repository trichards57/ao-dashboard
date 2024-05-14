// -----------------------------------------------------------------------
// <copyright file="KeyDates.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Data;

/// <summary>
/// Represents key dates stored in the database.
/// </summary>
public class KeyDates
{
    /// <summary>
    /// Gets or sets the ID of the key dates.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the last update file received.
    /// </summary>
    public DateOnly LastUpdateFile { get; set; }
}
