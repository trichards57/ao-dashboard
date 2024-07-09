// -----------------------------------------------------------------------
// <copyright file="SjaEmailAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Validation;

/// <summary>
/// Validates that the email address is a valid SJA email address.
/// </summary>
public class SjaEmailAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string email && !email.EndsWith("@sja.org.uk"))
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName != null ? [validationContext.MemberName] : null);
        }

        return ValidationResult.Success;
    }
}
