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
using Dashboard.Components;
using Dashboard.Components.Account;
using Dashboard.Data;
using Dashboard.Services;
using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? throw new InvalidOperationException("No Microsoft Client ID");
    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? throw new InvalidOperationException("No Microsoft Client Secret");
    microsoftOptions.AuthorizationEndpoint = "https://login.microsoftonline.com/91d037fb-4714-4fe8-b084-68c083b8193f/oauth2/v2.0/authorize";
    microsoftOptions.TokenEndpoint = "https://login.microsoftonline.com/91d037fb-4714-4fe8-b084-68c083b8193f/oauth2/v2.0/token";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Stores.MaxLengthForKeys = 128;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<ApplicationSignInManager>()
    .AddUserManager<ApplicationUserManager>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<AccountUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = "Dashboard-Auth";
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

const string LocalScheme = "LocalScheme";

builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "Dashboard-XSRF";
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.Configure<CookieAuthenticationOptions>(
    IdentityConstants.TwoFactorRememberMeScheme,
    c => { c.Cookie.Name = "Dashboard-2FA"; });
builder.Services.Configure<CookieAuthenticationOptions>(
    IdentityConstants.TwoFactorUserIdScheme,
    c => { c.Cookie.Name = "Dashboard-2FA-ID"; });

builder.Services.AddAuthentication(LocalScheme)
    .AddPolicyScheme(LocalScheme, "Either Authorization bearer Header or Auth Cookie", o =>
    {
        o.ForwardDefaultSelector = c =>
        {
            var authHeader = c.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            }

            return IdentityConstants.ApplicationScheme;
        };
    });

builder.Services.AddSingleton<IEmailSender, IdentityEmailSender>();
builder.Services.Configure<IdentityEmailSenderOptions>(builder.Configuration);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["HostUrl"] ?? throw new InvalidOperationException("No HostUrl configured.")),
    });

builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPlaceService, PlaceService>();
builder.Services.AddTransient<Dashboard.Services.IVehicleService, VehicleService>();
builder.Services.AddTransient<Dashboard.Client.Services.IVehicleService, VehicleService>();
builder.Services.AddTransient<IVorService, VorService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicies();
});

builder.Services.AddOpenIddict()
    .AddCore(o =>
    {
        o.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
        o.UseQuartz();
    })
    .AddServer(o =>
    {
        o.SetTokenEndpointUris("/connect/token");
        o.SetRevocationEndpointUris("/connect/revoke");
        o.AllowClientCredentialsFlow();
        o.AddEphemeralEncryptionKey();
        o.AddEphemeralSigningKey();
        o.UseAspNetCore().EnableTokenEndpointPassthrough();
    })
    .AddValidation(o =>
    {
        o.UseLocalServer();
        o.UseAspNetCore();
    });

builder.Services.AddHostedService<OpenIdWorker>();
builder.Services.AddOptions<OpenIdWorkerSettings>().BindConfiguration("OpenIdWorkerSettings");

builder.Services.AddQuartz(o =>
{
    o.UseSimpleTypeLoader();
    o.UseInMemoryStore();
}).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

builder.Services.AddApplicationInsightsTelemetry(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.DeveloperMode = true;
    }

    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddSingleton<ITelemetryInitializer, AppInsightsTelemetryInitializer>();

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddApplicationStatus()
    .AddApplicationInsightsPublisher(builder.Configuration["ApplicationInsights:ConnectionString"]);

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
                { "ai.cloud.roleInstance", "Server-" + (builder.Environment.IsDevelopment() ? "Development" : "Production") },
            },
        };

        await a.AddTelemetryInitializer(t);
    });

if (!string.IsNullOrWhiteSpace(builder.Configuration["ApplicationInsights:ConnectionString"]))
{
    builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
        configureApplicationInsightsLoggerOptions: (options) => { });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Dashboard.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
