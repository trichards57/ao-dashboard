using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Security.Claims;

namespace Dashboard.Services;

public class AppInsightsTelemetryInitializer(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : ITelemetryInitializer
{
    private readonly IWebHostEnvironment environment = environment;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public void Initialize(ITelemetry telemetry)
    {
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            telemetry.Context.Cloud.RoleName = "AO-Dashboard-Server";
            telemetry.Context.Cloud.RoleInstance = environment.IsDevelopment() ? "Development" : "Production";
        }

        var context = httpContextAccessor.HttpContext;

        if (context != null)
        {
            telemetry.Context.User.AuthenticatedUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
