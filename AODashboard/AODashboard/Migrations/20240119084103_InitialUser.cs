// -----------------------------------------------------------------------
// <copyright file="20240119084103_InitialUser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AODashboard.Migrations
{
    /// <inheritdoc />
    public partial class InitialUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Log",
                keyColumn: "Id",
                keyValue: new Guid("74cfc28d-82a7-4d0d-bab8-16e9490118a4"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("24149693-21eb-408a-a244-1e3ab38fd6af"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d1d247ee-5dd5-4fe4-b74b-200baeee87a8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e5099954-4cc0-4465-9b33-ad1421fc150d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ef7361d4-edb6-417a-b4f8-63bb48fadaf9"));

            migrationBuilder.InsertData(
                table: "Log",
                columns: new[] { "Id", "Action", "Reason", "TimeStamp", "UserId" },
                values: new object[] { new Guid("95f8f39c-7621-4c75-a3ad-aaefb0d807c7"), "Initial Setup", "Migration Run", new DateTimeOffset(new DateTime(2024, 1, 19, 8, 41, 3, 17, DateTimeKind.Unspecified).AddTicks(6630), new TimeSpan(0, 0, 0, 0, 0)), "EF Migrations" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Permissions", "SensitivePermissions", "VehicleConfiguration", "VorData" },
                values: new object[,]
                {
                    { new Guid("10e21ec1-ec61-4cf9-a61c-8dee0d47f3ab"), "LAL", 0, 0, 1, 1 },
                    { new Guid("872c8d27-13ee-4805-9604-fba55bd26477"), "DAL", 0, 0, 2, 2 },
                    { new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73"), "Administrator", 2, 2, 2, 2 },
                    { new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67"), "RAL", 2, 0, 2, 2 },
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73"), "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw" },
                    { new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67"), "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Log",
                keyColumn: "Id",
                keyValue: new Guid("95f8f39c-7621-4c75-a3ad-aaefb0d807c7"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10e21ec1-ec61-4cf9-a61c-8dee0d47f3ab"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("872c8d27-13ee-4805-9604-fba55bd26477"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73"), "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw" });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67"), "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw" });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67"));

            migrationBuilder.InsertData(
                table: "Log",
                columns: new[] { "Id", "Action", "Reason", "TimeStamp", "UserId" },
                values: new object[] { new Guid("74cfc28d-82a7-4d0d-bab8-16e9490118a4"), "Initial Setup", "Migration Run", new DateTimeOffset(new DateTime(2024, 1, 18, 12, 55, 36, 202, DateTimeKind.Unspecified).AddTicks(9861), new TimeSpan(0, 0, 0, 0, 0)), "EF Migrations" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Permissions", "SensitivePermissions", "VehicleConfiguration", "VorData" },
                values: new object[,]
                {
                    { new Guid("24149693-21eb-408a-a244-1e3ab38fd6af"), "LAL", 0, 0, 1, 1 },
                    { new Guid("d1d247ee-5dd5-4fe4-b74b-200baeee87a8"), "RAL", 2, 0, 2, 2 },
                    { new Guid("e5099954-4cc0-4465-9b33-ad1421fc150d"), "DAL", 0, 0, 2, 2 },
                    { new Guid("ef7361d4-edb6-417a-b4f8-63bb48fadaf9"), "Administrator", 2, 2, 2, 2 },
                });
        }
    }
}
