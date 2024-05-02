// -----------------------------------------------------------------------
// <copyright file="IPlaceService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;

namespace Dashboard.Client.Services;

public interface IPlaceService
{
    IAsyncEnumerable<string> GetDistricts(Region region);
    IAsyncEnumerable<string> GetHubs(Region region, string district);
}
