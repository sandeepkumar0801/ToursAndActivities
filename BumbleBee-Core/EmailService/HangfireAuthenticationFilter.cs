using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;

public class HangfireBasicAuthenticationFilter : IDashboardAuthorizationFilter
{

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        if(!httpContext.User.Identity.IsAuthenticated)
        {
            httpContext.Response.Redirect("https://localhost:7052/Account/Login");
            return false;

        }
        return true;
    }
}