// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using AODashboard.Client.Auth;
using AODashboard.Client.Services;
using AODashboard.Components;
using AODashboard.Components.Account;
using AODashboard.Data;
using AODashboard.Middleware;
using AODashboard.Middleware.ServerTiming;
using AODashboard.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using NetEscapades.AspNetCore.SecurityHeaders;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Syncfusion.Blazor;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionLicense"]);

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
        o.Scope.Add("offline_access");
        o.Scope.Add("User.Read");
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
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
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
    o.OperationFilter<SecurityRequirementsOperationFilter>();
    o.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    o.OperationFilter<AddResponseHeadersFilter>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCaching();
builder.Services.AddServerTiming();

builder.Services.AddOptions<SwaggerUIOptions>()
    .Configure<IHttpContextAccessor>((o, e) =>
    {
        var originalIndexStreamFactory = o.IndexStream;

        // 3. Override the Stream factory
        o.IndexStream = () =>
        {
            // 4. Read the original index.html file
            using var originalStream = originalIndexStreamFactory();
            using var originalStreamReader = new StreamReader(originalStream);
            var originalIndexHtmlContents = originalStreamReader.ReadToEnd();

            // 5. Get the request-specific nonce generated by NetEscapades.AspNetCore.SecurityHeaders
            var requestSpecificNonce = e.HttpContext!.GetNonce();

            // 6. Replace inline `<script>` and `<style>` tags by adding a `nonce` attribute to them
            var nonceEnabledIndexHtmlContents = originalIndexHtmlContents
                .Replace("<script>", $"<script nonce=\"{requestSpecificNonce}\">", StringComparison.OrdinalIgnoreCase)
                .Replace("<style>", $"<style nonce=\"{requestSpecificNonce}\">", StringComparison.OrdinalIgnoreCase);

            // 7. Return a new Stream that contains our modified contents
            return new MemoryStream(Encoding.UTF8.GetBytes(nonceEnabledIndexHtmlContents));
        };
    });

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddMicrosoftGraph();
builder.Services.AddMudServices();
builder.Services.AddSyncfusionBlazor();

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

builder.Services.AddAntiforgery(o =>
{
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddApplicationInsightsTelemetry(o =>
{
    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

var app = builder.Build();

var headerPolicy = new HeaderPolicyCollection();

var cspSettings = builder.Configuration.GetSection("CSP").Get<CspSettings>() ?? new();

headerPolicy
    .AddXssProtectionDisabled()
    .AddContentTypeOptionsNoSniff()
    .AddReferrerPolicyNoReferrer()
    .RemoveServerHeader()
    .AddFrameOptionsDeny()
    .AddContentSecurityPolicy(csp =>
    {
        csp.AddBlockAllMixedContent();
        csp.AddStyleSrc().Self().From(cspSettings.StyleSource).UnsafeInline();
        csp.AddFontSrc().Self().From(cspSettings.FontSource).Data();
        csp.AddFormAction().Self();
        csp.AddFrameAncestors().None();
        csp.AddImgSrc().Self().Data();
        csp.AddScriptSrc().Self().WithNonce().WasmUnsafeEval();
        csp.AddBaseUri().Self();
        csp.AddDefaultSrc().Self();
        csp.AddUpgradeInsecureRequests();
        csp.AddConnectSrc().Self().From(cspSettings.ConnectSource);
    });

app.UseSecurityHeaders(headerPolicy);
app.UseMiddleware<ServerTimingMiddleware>();

app.UseResponseCaching();

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
