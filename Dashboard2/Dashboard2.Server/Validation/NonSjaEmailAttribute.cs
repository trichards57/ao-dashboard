// -----------------------------------------------------------------------
// <copyright file="NonSjaEmailAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Validation;

/// <summary>
/// A validation attribute to ensure that an email address is not an SJA email address.
/// </summary>
public class NonSjaEmailAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string email && email.EndsWith("@sja.org.uk"))
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName != null ? [validationContext.MemberName] : null);
        }

        return ValidationResult.Success;
    }
}
