// -----------------------------------------------------------------------
// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceDashboard.Controllers;

/// <summary>
/// Controller for handling user operations.
/// </summary>
[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
}
