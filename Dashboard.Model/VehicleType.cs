﻿// -----------------------------------------------------------------------
// <copyright file="VehicleType.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Model;

public enum VehicleType
{
    /// <summary>
    /// Any other vehicle.
    /// </summary>
    Other = 0,

    /// <summary>
    /// A front-line ambulance.
    /// </summary>
    FrontLineAmbulance = 1,

    /// <summary>
    /// An all-wheel-drive ambulance.
    /// </summary>
    AllWheelDrive = 2,

    /// <summary>
    /// An off-road ambulance.
    /// </summary>
    OffRoadAmbulance = 3,
}
