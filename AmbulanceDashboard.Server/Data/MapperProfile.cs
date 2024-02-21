// -----------------------------------------------------------------------
// <copyright file="MapperProfile.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Model;

namespace AmbulanceDashboard.Data;

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
        CreateMap<Vehicle, VorStatus>()
            .ForMember(v => v.DueBack, o => o.MapFrom(v => (!v.IsVor || v.Incidents.OrderByDescending(v => v.StartDate).FirstOrDefault() == null) ? null : v.Incidents.OrderByDescending(v => v.StartDate).First().EstimatedEndDate))
            .ForMember(v => v.Summary, o => o.MapFrom(v => (!v.IsVor || v.Incidents.OrderByDescending(v => v.StartDate).FirstOrDefault() == null) ? null : v.Incidents.OrderByDescending(v => v.StartDate).First().Description));

        CreateMap<Vehicle, VehicleNames>();
        CreateMap<Vehicle, VehicleSettings>();

        CreateMap<UpdateVehicleSettings, Vehicle>()
            .ForMember(n => n.LastModified, (n) => n.MapFrom(m => DateTimeOffset.UtcNow))
            .ForMember(v => v.VehicleType, o => o.MapFrom(v => v.VehicleType));
    }
}
