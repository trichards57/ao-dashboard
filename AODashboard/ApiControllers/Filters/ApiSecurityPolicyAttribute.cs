using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace AODashboard.ApiControllers.Filters;

public class ApiSecurityPolicyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        context.HttpContext.Response.Headers.ContentSecurityPolicy = new StringValues("frame-ancestors 'none'");
        context.HttpContext.Response.Headers.XContentTypeOptions = new StringValues("nosniff");
        context.HttpContext.Response.Headers.XFrameOptions = new StringValues("DENY");
    }
}
