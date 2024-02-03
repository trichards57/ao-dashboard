// -----------------------------------------------------------------------
// <copyright file="20240129092821_VehicleIndexes.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AODashboard.Migrations;

/// <inheritdoc />
public partial class VehicleIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateIndex(name: "IX_Vehicles_Registration", table: "Vehicles", column: "Registration", unique: true);

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropIndex(name: "IX_Vehicles_Registration", table: "Vehicles");
}
