// -----------------------------------------------------------------------
// <copyright file="20240418083055_DataInitialisation.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dashboard.Migrations
{
    /// <inheritdoc />
    public partial class DataInitialisation : Migration
    {
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Registration",
                table: "Vehicles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "872C8D27-13EE-4805-9604-FBA55BD26477");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "91D78E3D-3170-4057-A6ED-6A78E84B2E73", "55BFD5CA-BFD4-4833-8790-4177D5C895A4" });

            migrationBuilder.DeleteData(
                table: "KeyDates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91D78E3D-3170-4057-A6ED-6A78E84B2E73");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "55BFD5CA-BFD4-4833-8790-4177D5C895A4");

            migrationBuilder.InsertData(
                table: "Log",
                columns: ["Id", "Action", "Reason", "TimeStamp", "UserId"],
                values: [Guid.NewGuid(), "Reverted DataInitialisation migration.", "Data Schema changed.", DateTimeOffset.UtcNow, "EF Core Migrations"]);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.Sql("DENY UPDATE, DELETE ON Log TO PUBLIC;");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB", null, "LAL", "LAL" },
                    { "872C8D27-13EE-4805-9604-FBA55BD26477", null, "DAL", "DAL" },
                    { "91D78E3D-3170-4057-A6ED-6A78E84B2E73", null, "Administrator", "ADMINISTRATOR" },
                    { "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67", null, "RAL", "RAL" },
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: ["Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RealName", "SecurityStamp", "TwoFactorEnabled", "UserName"],
                values: ["55BFD5CA-BFD4-4833-8790-4177D5C895A4", 0, "4dc01e2c-cb54-4269-b659-168f66d4ff5a", "trichards57@pm.me", true, false, null, "TRICHARDS57@PM.ME", "TRICHARDS57@PM.ME", null, null, false, "Tony Richards (Admin)", "5cdd4d51-2b48-464f-96df-e810d83d049b", false, "trichards57@pm.me"]);

            migrationBuilder.InsertData(
                table: "KeyDates",
                columns: ["Id", "LastUpdateFile"],
                values: [1, new DateOnly(1990, 1, 1)]);

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: ["RoleId", "UserId"],
                values: ["91D78E3D-3170-4057-A6ED-6A78E84B2E73", "55BFD5CA-BFD4-4833-8790-4177D5C895A4"]);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Registration",
                table: "Vehicles",
                column: "Registration",
                unique: true);

            migrationBuilder.InsertData(
                table: "Log",
                columns: ["Id", "Action", "Reason", "TimeStamp", "UserId"],
                values: [Guid.NewGuid(), "Applied DataInitialisation migration.", "Data Schema changed.", DateTimeOffset.UtcNow, "EF Core Migrations"]);
        }
    }
}
