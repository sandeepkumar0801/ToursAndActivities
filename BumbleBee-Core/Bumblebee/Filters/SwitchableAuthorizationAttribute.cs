using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Filters
{
    public class SwitchableAuthorizationAttribute : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public SwitchableAuthorizationAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (_configuration.GetValue<string>("IsAuthenticationApply") == "1")
                {
                    var CurrentIdentity = context.HttpContext.User.Identity;
                    if (!CurrentIdentity.IsAuthenticated)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                }
            }
            catch
            {
                // Handle the exception, or do nothing if you want to allow access in case of any error.
            }
        }
    }
}
