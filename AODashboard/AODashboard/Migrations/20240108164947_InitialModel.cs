// -----------------------------------------------------------------------
// <copyright file="20240108164947_InitialModel.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

namespace AODashboard.Migrations;

/// <inheritdoc />
public partial class InitialModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CallSign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Registration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BodyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsVor = table.Column<bool>(type: "bit", nullable: false),
                District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Region = table.Column<int>(type: "int", nullable: false),
                VehicleType = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Incidents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                EstimatedEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Incidents", x => x.Id);
                table.ForeignKey(
                    name: "FK_Incidents_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Incidents_VehicleId",
            table: "Incidents",
            column: "VehicleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Incidents");

        migrationBuilder.DropTable(
            name: "Vehicles");
    }
}
