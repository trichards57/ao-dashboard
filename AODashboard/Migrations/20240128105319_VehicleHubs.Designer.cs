﻿// <auto-generated />
using System;
using AODashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AODashboard.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240128105319_VehicleHubs")]
    partial class VehicleHubs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AODashboard.Data.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Log");

                    b.HasData(
                        new
                        {
                            Id = new Guid("cc0cb0fd-eb11-467a-a01f-d92357249a6b"),
                            Action = "Initial Setup",
                            Reason = "Migration Run",
                            TimeStamp = new DateTimeOffset(new DateTime(2024, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            UserId = "EF Migrations"
                        });
                });

            modelBuilder.Entity("AODashboard.Data.Incident", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("EstimatedEndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("Incidents");
                });

            modelBuilder.Entity("AODashboard.Data.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Permissions")
                        .HasColumnType("int");

                    b.Property<int>("SensitivePermissions")
                        .HasColumnType("int");

                    b.Property<int>("VehicleConfiguration")
                        .HasColumnType("int");

                    b.Property<int>("VorData")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73"),
                            Name = "Administrator",
                            Permissions = 2,
                            SensitivePermissions = 2,
                            VehicleConfiguration = 2,
                            VorData = 2
                        },
                        new
                        {
                            Id = new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67"),
                            Name = "RAL",
                            Permissions = 2,
                            SensitivePermissions = 0,
                            VehicleConfiguration = 2,
                            VorData = 2
                        },
                        new
                        {
                            Id = new Guid("872c8d27-13ee-4805-9604-fba55bd26477"),
                            Name = "DAL",
                            Permissions = 0,
                            SensitivePermissions = 0,
                            VehicleConfiguration = 2,
                            VorData = 2
                        },
                        new
                        {
                            Id = new Guid("10e21ec1-ec61-4cf9-a61c-8dee0d47f3ab"),
                            Name = "LAL",
                            Permissions = 0,
                            SensitivePermissions = 0,
                            VehicleConfiguration = 1,
                            VorData = 1
                        });
                });

            modelBuilder.Entity("AODashboard.Data.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            UserId = "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw",
                            RoleId = new Guid("91d78e3d-3170-4057-a6ed-6a78e84b2e73")
                        },
                        new
                        {
                            UserId = "0W2LTE_Dd_eIZdhlqItCbJdjYHTDnbX7nk1IzyaBlGw",
                            RoleId = new Guid("ae832a97-cdde-4c7d-aad3-16943feb7e67")
                        });
                });

            modelBuilder.Entity("AODashboard.Data.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BodyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CallSign")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hub")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVor")
                        .HasColumnType("bit");

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Region")
                        .HasColumnType("int");

                    b.Property<string>("Registration")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("AODashboard.Data.Incident", b =>
                {
                    b.HasOne("AODashboard.Data.Vehicle", "Vehicle")
                        .WithMany("Incidents")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("AODashboard.Data.UserRole", b =>
                {
                    b.HasOne("AODashboard.Data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("AODashboard.Data.Vehicle", b =>
                {
                    b.Navigation("Incidents");
                });
#pragma warning restore 612, 618
        }
    }
}