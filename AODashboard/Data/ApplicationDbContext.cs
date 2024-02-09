// -----------------------------------------------------------------------
// <copyright file="ApplicationDbContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AODashboard.Data;

#nullable disable

/// <summary>
/// Main data context used by the system.
/// </summary>
/// <param name="options">The database options for this instance.</param>
internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the incidents logged in the system.
    /// </summary>
    public DbSet<Incident> Incidents { get; set; }

    /// <summary>
    /// Gets or sets the vehicles logged in the system.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

    /// <summary>
    /// Gets or sets the roles available in the system.
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Gets or sets the user roles available in the system.
    /// </summary>
    public DbSet<UserRole> UserRoles { get; set; }

    /// <summary>
    /// Gets or sets the audit log.
    /// </summary>
    public DbSet<AuditLog> Log { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var adminGuid = new Guid("91D78E3D-3170-4057-A6ED-6A78E84B2E73");
        var ralGuid = new Guid("AE832A97-CDDE-4C7D-AAD3-16943FEB7E67");
        var dalGuid = new Guid("872C8D27-13EE-4805-9604-FBA55BD26477");
        var lalGuid = new Guid("10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB");
        var auditGuid = new Guid("CC0CB0FD-EB11-467A-A01F-D92357249A6B");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminGuid, Name = "Administrator", SensitivePermissions = PermissionLevel.ReadWrite, Permissions = PermissionLevel.ReadWrite, VehicleConfiguration = PermissionLevel.ReadWrite, VorData = PermissionLevel.ReadWrite },
            new Role { Id = ralGuid, Name = "RAL", SensitivePermissions = PermissionLevel.Forbid, Permissions = PermissionLevel.ReadWrite, VehicleConfiguration = PermissionLevel.ReadWrite, VorData = PermissionLevel.ReadWrite },
            new Role { Id = dalGuid, Name = "DAL", SensitivePermissions = PermissionLevel.Forbid, Permissions = PermissionLevel.Forbid, VehicleConfiguration = PermissionLevel.ReadWrite, VorData = PermissionLevel.ReadWrite },
            new Role { Id = lalGuid, Name = "LAL", SensitivePermissions = PermissionLevel.Forbid, Permissions = PermissionLevel.Forbid, VehicleConfiguration = PermissionLevel.Read, VorData = PermissionLevel.Read });

        modelBuilder.Entity<AuditLog>().HasData(
            new AuditLog { Id = auditGuid, Action = "Initial Setup", Reason = "Migration Run", TimeStamp = new DateTimeOffset(new DateTime(2024, 1, 24)), UserId = "EF Migrations" });

        modelBuilder.Entity<UserRole>()
            .HasKey(u => new { u.UserId, u.RoleId });

        // Configure Tony Richards to have RAL and Admin access.
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw", RoleId = adminGuid },
            new UserRole { UserId = "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw", RoleId = ralGuid });

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.Registration)
            .IsUnique();
    }
}
