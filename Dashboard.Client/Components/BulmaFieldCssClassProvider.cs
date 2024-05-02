// -----------------------------------------------------------------------
// <copyright file="BulmaFieldCssClassProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Client.Components;

/// <summary>
/// Field CSS class provider for Bulma validation fields.
/// </summary>
public class BulmaFieldCssClassProvider : FieldCssClassProvider
{
    /// <inheritdoc/>
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isValid = editContext.IsValid(fieldIdentifier);

        if (!isValid)
        {
            return "is-danger";
        }

        if (editContext.IsModified(fieldIdentifier))
        {
            return "is-success";
        }

        return "";
    }
}
