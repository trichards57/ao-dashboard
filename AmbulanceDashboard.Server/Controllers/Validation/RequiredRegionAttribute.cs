﻿// -----------------------------------------------------------------------
// <copyright file="RequiredRegionAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Data;
using AmbulanceDashboard.Model;
using System.ComponentModel.DataAnnotations;

namespace AmbulanceDashboard.Controllers.Validation;

/// <summary>
/// Validates that the provided value is both a valid region and not set to <see cref="Region.Unknown"/>.
/// </summary>
public sealed class RequiredRegionAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is not Region region)
        {
            throw new InvalidOperationException();
        }

        return Enum.IsDefined(region) && region != Region.Unknown;
    }
}