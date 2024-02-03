// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client;
using AODashboard.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA4MTY3MEAzMjM0MmUzMDJlMzBFUlloMk9CYXpGNGkxZHpEeWdmUVpTT0s5N29DcmQ4azdlcVRaWTcwaGR3PQ==");

builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicies();
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddSingleton<IVehicleService, VehicleService>();
builder.Services.AddSingleton<IPlaceService, PlaceService>();

builder.Services.AddSingleton((IServiceProvider s) =>
{
    var navManager = s.GetRequiredService<NavigationManager>();

    return new HttpClient()
    {
        BaseAddress = new Uri(navManager.BaseUri),
    };
});
builder.Services.AddMudServices();
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();
