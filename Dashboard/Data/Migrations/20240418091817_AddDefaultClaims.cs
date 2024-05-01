// -----------------------------------------------------------------------
// <copyright file="20240418091817_AddDefaultClaims.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: ["Id", "ClaimType", "ClaimValue", "RoleId"],
                values: new object[,]
                {
                    { 1, "Permissions", "Edit", "91D78E3D-3170-4057-A6ED-6A78E84B2E73" },
                    { 2, "Permissions", "Edit", "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67" },
                    { 3, "VehicleConfiguration", "Edit", "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67" },
                    { 4, "VORData", "Read", "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67" },
                    { 5, "Permissions", "Read", "872C8D27-13EE-4805-9604-FBA55BD26477" },
                    { 6, "VehicleConfiguration", "Read", "872C8D27-13EE-4805-9604-FBA55BD26477" },
                    { 7, "VORData", "Read", "872C8D27-13EE-4805-9604-FBA55BD26477" },
                    { 8, "VehicleConfiguration", "Read", "10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB" },
                    { 9, "VORData", "Read", "10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB" },
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "55BFD5CA-BFD4-4833-8790-4177D5C895A4",
                columns: ["ConcurrencyStamp", "SecurityStamp"],
                values: ["22b18e57-1b53-4d69-adf4-29e94689b409", "cf98f09a-4e97-4e6d-975c-2ebcecc470bb"]);

            migrationBuilder.InsertData(
                table: "Log",
                columns: ["Id", "Action", "Reason", "TimeStamp", "UserId"],
                values: [Guid.NewGuid(), "Applied AddDefaultClaims migration.", "Data Schema changed.", DateTimeOffset.UtcNow, "EF Core Migrations"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "55BFD5CA-BFD4-4833-8790-4177D5C895A4",
                columns: ["ConcurrencyStamp", "SecurityStamp"],
                values: ["4dc01e2c-cb54-4269-b659-168f66d4ff5a", "5cdd4d51-2b48-464f-96df-e810d83d049b"]);

            migrationBuilder.InsertData(
                table: "Log",
                columns: ["Id", "Action", "Reason", "TimeStamp", "UserId"],
                values: [Guid.NewGuid(), "Reverted AddDefaultClaims migration.", "Data Schema changed.", DateTimeOffset.UtcNow, "EF Core Migrations"]);
        }
    }
}
