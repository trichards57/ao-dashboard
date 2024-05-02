// -----------------------------------------------------------------------
// <copyright file="BooleanRequiredAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Validation;

/// <summary>
/// Validates that a boolean value is true.
/// </summary>
public class BooleanRequiredAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not bool b || !b)
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName != null ? [validationContext.MemberName] : null);
        }

        return ValidationResult.Success;
    }
}
