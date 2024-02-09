// -----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S6562:Always set the \"DateTimeKind\" when creating new \"DateTime\" instances", Justification = "Not required for this initial data.", Scope = "member", Target = "~M:AODashboard.Data.ApplicationDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "This is not run very often, not worth the change.", Scope = "member", Target = "~M:AODashboard.Migrations.InitialData.Up(Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder)")]
[assembly: SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "This is not run very often, not worth the change.", Scope = "member", Target = "~M:AODashboard.Migrations.InitialData.Up(Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder)")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetByCallSignAsync(System.String)~System.Threading.Tasks.Task{System.Nullable{AODashboard.Client.Model.VehicleSettings}}")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetByRegistrationAsync(System.String)~System.Threading.Tasks.Task{System.Nullable{AODashboard.Client.Model.VehicleSettings}}")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetEtagByCallSignAsync(System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetEtagByRegistrationAsync(System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetLastUpdateByCallSignAsync(System.String)~System.Threading.Tasks.Task{System.Nullable{System.DateTimeOffset}}")]
[assembly: SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "This would break the Entity Framework translation.", Scope = "member", Target = "~M:AODashboard.Services.VehicleService.GetLastUpdateByRegistration(System.String)~System.Threading.Tasks.Task{System.Nullable{System.DateTimeOffset}}")]
