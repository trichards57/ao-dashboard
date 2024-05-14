// -----------------------------------------------------------------------
// <copyright file="OpenIdWorkerSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Services;

/// <summary>
/// Represents the settings for the built-in Open ID clients.
/// </summary>
public class OpenIdWorkerSettings
{
    /// <summary>
    /// Gets or sets the ID for the VOR Uploader.
    /// </summary>
    public required string VorUploaderClientId { get; set; }

    /// <summary>
    /// Gets or sets the secret for the VOR Uploader.
    /// </summary>
    public required string VorUploaderClientSecret { get; set; }
}
