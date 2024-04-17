// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AmbulanceDashboard.Data;
using AmbulanceDashboard.Middleware.ServerTiming;
using AmbulanceDashboard.Services;
using AODashboard.Client.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPlaceService, PlaceService>();
builder.Services.AddTransient<IVehicleService, VehicleService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

const string LocalScheme = "LocalScheme";

builder.Services
    .AddAuthentication(LocalScheme)
    .AddPolicyScheme(LocalScheme, "Either Authorization Bearer header or Auth Cookie", o =>
    {
        o.ForwardDefaultSelector = c =>
        {
            var authHeader = c.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }

            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    }).AddMicrosoftIdentityWebApi(
        o =>
        {
            builder.Configuration.Bind("AzureAd", o);
            o.TokenValidationParameters.ValidAudiences = [o.Audience, $"api://{o.Audience}"];
            o.Events = new JwtBearerEvents
            {
                OnTokenValidated = async c =>
                {
                    var service = c.HttpContext.RequestServices.GetRequiredService<IUserService>();
                    var userId = c.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not available.");

                    var claims = await service.GetClaimsAsync(userId).ToListAsync();

                    c.Principal.AddIdentity(new ClaimsIdentity(claims, "Local"));
                },
            };
        },
        o =>
        {
            builder.Configuration.Bind("AzureAd", o);
        });

builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(
    o =>
    {
        builder.Configuration.Bind("AzureAd", o);
        o.Events.OnTokenValidated = async c =>
        {
            var service = c.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var userId = c.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not available.");

            var claims = await service.GetClaimsAsync(userId).ToListAsync();

            c.Principal.AddIdentity(new ClaimsIdentity(claims, "Local"));
        };
    },
    o =>
    {
        o.Events.OnRedirectToLogin = c =>
        {
            c.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicies();
});

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddServerTiming();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.Context.Request.Path.StartsWithSegments("/assets"))
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000, immutable");
        }
    },
});
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();