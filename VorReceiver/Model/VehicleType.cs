// -----------------------------------------------------------------------
// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace VorReceiver.Model;

/// <summary>
/// Represents the type of the vehicle.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VehicleType
{
    /// <summary>
    /// Any other vehicle.
    /// </summary>
    [EnumMember(Value ="other")]
    Other = 0,
    
    /// <summary>
    /// A front-line ambulance.
    /// </summary>
    [EnumMember(Value = "frontline")]
    FrontLineAmbulance = 1,

    /// <summary>
    /// An all-wheel-drive ambulance.
    /// </summary>
    [EnumMember(Value = "awd")]
    AllWheelDrive = 2,

    /// <summary>
    /// An off-road ambulance.
    /// </summary>
    [EnumMember(Value = "ora")]
    OffRoadAmbulance = 3,
}
