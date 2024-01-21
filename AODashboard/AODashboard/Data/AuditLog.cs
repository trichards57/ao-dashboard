// -----------------------------------------------------------------------
// <copyright file="AuditLog.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Data;

/// <summary>
/// Represents an entry added to the audit log.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Gets or sets the time the log entry was made.
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the ID of the log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the action being logged.
    /// </summary>
    public string Action { get; set; } = "";

    /// <summary>
    /// Gets or sets the ID of the user that completed the action.
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// Gets or sets the reason the activity was completed.
    /// </summary>
    public string Reason { get; set; } = "";
}
