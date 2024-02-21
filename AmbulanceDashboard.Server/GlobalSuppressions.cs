// -----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S6562:Always set the \"DateTimeKind\" when creating new \"DateTime\" instances", Justification = "Fixing this breaks Linq to SQL.", Scope = "member", Target = "~M:AmbulanceDashboard.Data.ApplicationDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)")]
