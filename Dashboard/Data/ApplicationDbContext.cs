// -----------------------------------------------------------------------
// <copyright file="ApplicationDbContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Data;

/// <summary>
/// Data context for the application.
/// </summary>
/// <param name="options">The database configuration options.</param>
internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>
    /// Gets or sets the incidents logged in the system.
    /// </summary>
    public DbSet<Incident> Incidents { get; set; }

    /// <summary>
    /// Gets or sets the key dates.
    /// </summary>
    public DbSet<KeyDates> KeyDates { get; set; }

    /// <summary>
    /// Gets or sets the vehicles logged in the system.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

    /// <summary>
    /// Gets or sets the audit log.
    /// </summary>
    public DbSet<AuditLog> Log { get; set; }

    /// <summary>
    /// Gets or sets the user invitations.
    /// </summary>
    public DbSet<UserInvite> UserInvites { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var adminGuid = "91D78E3D-3170-4057-A6ED-6A78E84B2E73";
        var adminUserGuid = "55BFD5CA-BFD4-4833-8790-4177D5C895A4";
        var ralGuid = "AE832A97-CDDE-4C7D-AAD3-16943FEB7E67";
        var dalGuid = "872C8D27-13EE-4805-9604-FBA55BD26477";
        var lalGuid = "10E21EC1-EC61-4CF9-A61C-8DEE0D47F3AB";

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = adminGuid, Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new IdentityRole { Id = ralGuid, Name = "RAL", NormalizedName = "RAL" },
            new IdentityRole { Id = dalGuid, Name = "DAL", NormalizedName = "DAL" },
            new IdentityRole { Id = lalGuid, Name = "LAL", NormalizedName = "LAL" });

        builder.Entity<IdentityRoleClaim<string>>().HasData(
            new IdentityRoleClaim<string> { Id = 1, RoleId = adminGuid, ClaimType = Client.UserClaims.Permissions, ClaimValue = Client.UserClaims.Edit },
            new IdentityRoleClaim<string> { Id = 2, RoleId = ralGuid, ClaimType = Client.UserClaims.Permissions, ClaimValue = Client.UserClaims.Edit },
            new IdentityRoleClaim<string> { Id = 3, RoleId = ralGuid, ClaimType = Client.UserClaims.VehicleConfiguration, ClaimValue = Client.UserClaims.Edit },
            new IdentityRoleClaim<string> { Id = 4, RoleId = ralGuid, ClaimType = Client.UserClaims.VorData, ClaimValue = Client.UserClaims.Read },
            new IdentityRoleClaim<string> { Id = 5, RoleId = dalGuid, ClaimType = Client.UserClaims.Permissions, ClaimValue = Client.UserClaims.Read },
            new IdentityRoleClaim<string> { Id = 6, RoleId = dalGuid, ClaimType = Client.UserClaims.VehicleConfiguration, ClaimValue = Client.UserClaims.Read },
            new IdentityRoleClaim<string> { Id = 7, RoleId = dalGuid, ClaimType = Client.UserClaims.VorData, ClaimValue = Client.UserClaims.Read },
            new IdentityRoleClaim<string> { Id = 8, RoleId = lalGuid, ClaimType = Client.UserClaims.VehicleConfiguration, ClaimValue = Client.UserClaims.Read },
            new IdentityRoleClaim<string> { Id = 9, RoleId = lalGuid, ClaimType = Client.UserClaims.VorData, ClaimValue = Client.UserClaims.Read });

        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = adminUserGuid,
                Email = "trichards57@pm.me",
                EmailConfirmed = true,
                NormalizedEmail = "TRICHARDS57@PM.ME",
                NormalizedUserName = "TRICHARDS57@PM.ME",
                UserName = "trichards57@pm.me",
                RealName = "Tony Richards (Admin)",
                ConcurrencyStamp = "ce3dc57d-0a22-4410-8415-9eef26c71fd8",
                SecurityStamp = "4fb358dd-0aff-4c04-b217-e99be9af112f",
            });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { RoleId = adminGuid, UserId = adminUserGuid });

        builder.Entity<Vehicle>()
            .HasIndex(v => v.Registration)
            .IsUnique();

        builder.Entity<KeyDates>()
            .HasData(new KeyDates { Id = 1, LastUpdateFile = new DateOnly(1990, 1, 1) });

        builder.Entity<UserInvite>()
            .HasIndex(v => v.Email)
            .IsUnique();
        builder.Entity<UserInvite>()
            .HasOne(u => u.Role).WithMany().HasForeignKey(i => i.RoleId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<UserInvite>()
            .HasOne(u => u.CreatedBy).WithMany().HasForeignKey(i => i.CreatedById).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<UserInvite>()
            .Property(u => u.Created).HasDefaultValueSql("GETUTCDATE()");
    }
}
