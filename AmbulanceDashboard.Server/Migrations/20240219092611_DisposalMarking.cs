// -----------------------------------------------------------------------
// <copyright file="20240219092611_DisposalMarking.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceDashboard.Migrations
{
    /// <inheritdoc />
    public partial class DisposalMarking : Migration
    {
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(name: "ForDisposal", table: "Vehicles");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<bool>(
                name: "ForDisposal",
                table: "Vehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);
    }
}
