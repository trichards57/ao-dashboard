// -----------------------------------------------------------------------
// <copyright file="MapperProfile.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;

namespace AODashboard.Data;

/// <summary>
/// Mapper used to convert between transfer and storage types.
/// </summary>
internal sealed class MapperProfile : AutoMapper.Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapperProfile"/> class.
    /// </summary>
    public MapperProfile()
    {
        CreateMap<Vehicle, VehicleSettings>();
        CreateMap<Vehicle, VorStatus>();

        CreateMap<UpdateVehicleSettings, Vehicle>()
            .ForMember(n => n.LastModified, (n) => n.MapFrom(m => DateTimeOffset.UtcNow));
    }
}
