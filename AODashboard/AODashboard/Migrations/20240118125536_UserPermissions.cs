// -----------------------------------------------------------------------
// <copyright file="20240118125536_UserPermissions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AODashboard.Migrations
{
    /// <inheritdoc />
    public partial class UserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleConfiguration = table.Column<int>(type: "int", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    SensitivePermissions = table.Column<int>(type: "int", nullable: false),
                    VorData = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
