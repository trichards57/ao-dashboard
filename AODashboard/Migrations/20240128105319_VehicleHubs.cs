// -----------------------------------------------------------------------
// <copyright file="20240128105319_VehicleHubs.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AODashboard.Migrations;

/// <inheritdoc />
public partial class VehicleHubs : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(name: "Hub", table: "Vehicles", type: "nvarchar(max)", nullable: false, defaultValue: "");

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(name: "Hub", table: "Vehicles");
}
