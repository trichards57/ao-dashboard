// -----------------------------------------------------------------------
// <copyright file="ApiSecurityPolicyAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace AODashboard.ApiControllers.Filters;

/// <summary>
/// Attribute to add the security headers to the response.
/// </summary>
public class ApiSecurityPolicyAttribute : ActionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        context.HttpContext.Response.Headers.ContentSecurityPolicy = new StringValues("frame-ancestors 'none'");
        context.HttpContext.Response.Headers.XContentTypeOptions = new StringValues("nosniff");
        context.HttpContext.Response.Headers.XFrameOptions = new StringValues("DENY");
    }
}
