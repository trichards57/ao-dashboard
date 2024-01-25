// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client;
using AODashboard.Client.Services;
using AODashboard.Components;
using AODashboard.Components.Account;
using AODashboard.Data;
using AODashboard.Services;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(o =>
    {
        builder.Configuration.Bind("AzureAd", o);
        o.Events.OnTokenValidated = async c =>
        {
            var service = c.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var userId = c.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID not available.");

            var claims = await service.GetClaimsAsync(userId).ToListAsync();

            c.Principal.AddIdentity(new ClaimsIdentity(claims, "Local"));
        };
    })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services
    .AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), JwtBearerDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstreamApi();

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.AccessDeniedPath = new PathString("/AccessDenied");
});

builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicies();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AO Dashboard API",
        Description = "Dashboard to access the data behind the AO Dashboard.",
        Contact = new OpenApiContact
        {
            Name = "Tony Richards",
            Email = "Tony.Richards@sja.org.uk",
        },
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(
    (s, a) =>
    {
        a.AddCollectionMappers();
        a.UseEntityFrameworkCoreModel<ApplicationDbContext>(s);
    },
    typeof(MapperProfile));

builder.Services.AddMicrosoftGraph();
builder.Services.AddMudServices();

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    o.AddPolicy("upload", partitioner: httpContext =>
    {
        var username = httpContext.User.Identity?.Name ?? string.Empty;

        if (!string.IsNullOrEmpty(username))
        {
            return RateLimitPartition.GetFixedWindowLimiter(username, o =>
            {
                var options = new FixedWindowRateLimiterOptions();
                builder.Configuration.Bind("UploadRateLimiting:Authenticated", options);
                return options;
            });
        }

        return RateLimitPartition.GetFixedWindowLimiter(username, o =>
        {
            var options = new FixedWindowRateLimiterOptions();
            builder.Configuration.Bind("UploadRateLimiting:Anonymous", options);
            return options;
        });
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AODashboard.Client._Imports).Assembly);

app.Run();
