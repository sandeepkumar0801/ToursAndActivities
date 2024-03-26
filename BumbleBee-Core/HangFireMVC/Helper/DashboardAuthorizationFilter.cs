using Hangfire.Dashboard;

namespace HangFireMVC.Helper
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check if the user is authenticated
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                // Redirect to the login page
                httpContext.Response.Redirect("/Identity/Account/Login");
                return false;
            }

            // Check if the user is authorized
            if (!httpContext.User.IsInRole("Manager"))
            {
                // Return HTTP 403 (Forbidden)
                httpContext.Response.StatusCode = 403;
                return false;
            }

            return true;
        }
    }

}
