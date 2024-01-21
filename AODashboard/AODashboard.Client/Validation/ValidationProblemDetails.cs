﻿// -----------------------------------------------------------------------
// <copyright file="ValidationProblemDetails.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace AODashboard.Client.Validation;

/// <summary>
/// A machine-readable format for specifying errors in HTTP API responses based on <see href="https://tools.ietf.org/html/rfc7807"/>.
/// </summary>
public class ValidationProblemDetails
{
    /// <summary>
    /// Gets or sets a human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("detail")]
    public string? Detail { get; set; } = default;

    /// <summary>
    /// Gets or sets the validation errors associated with this instance of <see cref="ValidationProblemDetails"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? Errors { get; set; } = [];

    /// <summary>
    /// Gets or sets a URI reference that identifies the specific occurrence of the problem. It may or may not yield further information if dereferenced.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("instance")]
    public string? Instance { get; set; } = default;

    /// <summary>
    /// Gets or sets the HTTP status code([RFC7231], Section 6) generated by the origin server for this occurrence of the problem.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("status")]
    public int? Status { get; set; } = default;

    /// <summary>
    /// Gets or sets a short, human-readable summary of the problem type. It SHOULD NOT change from occurrence to occurrence
    /// of the problem, except for purposes of localization(e.g., using proactive content negotiation;
    /// see[RFC7231], Section 3.4).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string? Title { get; set; } = default;

    /// <summary>
    /// Gets or sets a URI reference [RFC3986] that identifies the problem type. This specification encourages that, when
    /// dereferenced, it provide human-readable documentation for the problem type
    /// (e.g., using HTML [W3C.REC-html5-20141028]). When this member is not present, its value is assumed to be
    /// "about:blank".
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public string? Type { get; set; } = default;
}
