// -----------------------------------------------------------------------
// <copyright file="VehicleTypeConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model.Converters;

/// <summary>
/// Converts between <see cref="VehicleType"/> and <see cref="Grpc.VehicleType"/>.
/// </summary>
public static class VehicleTypeConverter
{
    /// <summary>
    /// Converts a <see cref="VehicleType"/> to a <see cref="Grpc.VehicleType"/>.
    /// </summary>
    /// <param name="vehicleType">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static Grpc.VehicleType ToGrpc(VehicleType vehicleType) => vehicleType switch
    {
        VehicleType.FrontLineAmbulance => Grpc.VehicleType.FrontLineAmbulance,
        VehicleType.AllWheelDrive => Grpc.VehicleType.AllWheelDrive,
        VehicleType.OffRoadAmbulance => Grpc.VehicleType.OffRoadAmbulance,
        VehicleType.Other => Grpc.VehicleType.Other,
        _ => Grpc.VehicleType.Undefined,
    };

    /// <summary>
    /// Converts a <see cref="Grpc.VehicleType"/> to a <see cref="VehicleType"/>.
    /// </summary>
    /// <param name="vehicleType">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static VehicleType ToData(Grpc.VehicleType vehicleType) => vehicleType switch
    {
        Grpc.VehicleType.FrontLineAmbulance => VehicleType.FrontLineAmbulance,
        Grpc.VehicleType.AllWheelDrive => VehicleType.AllWheelDrive,
        Grpc.VehicleType.OffRoadAmbulance => VehicleType.OffRoadAmbulance,
        _ => VehicleType.Other,
    };
}
