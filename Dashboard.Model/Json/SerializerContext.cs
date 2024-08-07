// -----------------------------------------------------------------------
// <copyright file="SerializerContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Model;
using System.Text.Json.Serialization;

namespace Dashboard.Model.Json;

[JsonSerializable(typeof(Place))]
[JsonSerializable(typeof(RolePermissions))]
[JsonSerializable(typeof(IAsyncEnumerable<RolePermissions>))]
[JsonSerializable(typeof(List<RolePermissions>))]
[JsonSerializable(typeof(RolePermissionsUpdate))]
[JsonSerializable(typeof(UserWithRole))]
[JsonSerializable(typeof(IAsyncEnumerable<UserWithRole>))]
[JsonSerializable(typeof(List<UserWithRole>))]
[JsonSerializable(typeof(VehicleSettings))]
[JsonSerializable(typeof(IAsyncEnumerable<VehicleSettings>))]
[JsonSerializable(typeof(List<VehicleSettings>))]
[JsonSerializable(typeof(UpdateVehicleSettings))]
[JsonSerializable(typeof(VorIncident))]
[JsonSerializable(typeof(UserInfo))]
[JsonSerializable(typeof(UserRoleUpdate))]
[JsonSerializable(typeof(VorStatistics))]
[JsonSerializable(typeof(VorStatus))]
[JsonSerializable(typeof(IAsyncEnumerable<VorStatus>))]
[JsonSerializable(typeof(List<VorStatus>))]
[JsonSerializable(typeof(IAsyncEnumerable<string>))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions]
public partial class SerializerContext : JsonSerializerContext
{

}
