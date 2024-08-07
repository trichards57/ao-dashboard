// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using BlazorApplicationInsights;
using BlazorApplicationInsights.Models;
using Dashboard.Client;
using Dashboard.Client.Services;
using Dashboard.Model.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (builder.HostEnvironment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}
else if (builder.HostEnvironment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Production.json");
}

builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicies();
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPlaceService, PlaceService>();
builder.Services.AddTransient<IVorService, VorService>();
builder.Services.AddTransient<IVehicleService, VehicleService>();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    TypeInfoResolver = SerializerContext.Default,
    PropertyNameCaseInsensitive = true,
});

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["HostUrl"] ?? throw new InvalidOperationException("No HostUrl configured.")),
    });

builder.Services.AddBlazorApplicationInsights(
    x =>
    {
        x.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    },
    async a =>
    {
        var t = new TelemetryItem()
        {
            Tags = new Dictionary<string, object?>
            {
                { "ai.cloud.role", "AO-Dashboard-Client" },
                { "ai.cloud.roleInstance", "client-" + (builder.HostEnvironment.IsDevelopment() ? "Development" : "Production") },
            },
        };

        await a.AddTelemetryInitializer(t);
    });

await builder.Build().RunAsync();
