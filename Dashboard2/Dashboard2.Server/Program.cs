// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Services;
using Dashboard.Data;
using Dashboard.Services;
using Dashboard2.Server.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPlaceService, PlaceService>();
builder.Services.AddTransient<IVehicleService, VehicleService>();
builder.Services.AddTransient<IVorService, VorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapConnect()
   .MapRoles()
   .MapUsers()
   .MapPlaces();

app.Run();
