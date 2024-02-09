// -----------------------------------------------------------------------
// <copyright file="InvalidRequestException.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace AODashboard.Client.Validation;

/// <summary>
/// Represents an exception raised because server-side validation has failed.
/// </summary>
public class InvalidRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestException"/> class.
    /// </summary>
    public InvalidRequestException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestException"/> class with the provided message.
    /// </summary>
    /// <param name="message">The message describing the exception.</param>
    public InvalidRequestException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestException"/> class.
    /// </summary>
    /// <param name="problemDetails">The details of the problem provided by the server.</param>
    /// <param name="message">The message describing the exception.</param>
    public InvalidRequestException(ValidationProblemDetails? problemDetails, string message)
        : base(message)
    {
        ProblemDetails = problemDetails;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestException"/> class.
    /// </summary>
    /// <param name="message">The message describing the exception.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets the details of the problem provided by the server.
    /// </summary>
    public ValidationProblemDetails? ProblemDetails { get; }
}
