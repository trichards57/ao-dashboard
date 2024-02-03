// -----------------------------------------------------------------------
// <copyright file="MapperProfile.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Model;
using AutoMapper.EquivalencyExpression;

namespace AODashboard.Data;

/// <summary>
/// Mapper used to convert between transfer and storage types.
/// </summary>
internal class MapperProfile : AutoMapper.Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapperProfile"/> class.
    /// </summary>
    public MapperProfile()
    {
        CreateMap<Vehicle, VehicleSettings>();
        CreateMap<Vehicle, VorStatus>();

        CreateMap<UpdateVehicleSettings, Vehicle>()
            .EqualityComparison((a, b) => a.Registration == b.Registration)
            .ForMember(n => n.LastModified, (n) => n.MapFrom(m => DateTimeOffset.UtcNow));
    }
}
