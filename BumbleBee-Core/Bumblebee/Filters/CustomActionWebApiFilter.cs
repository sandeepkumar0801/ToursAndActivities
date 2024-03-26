using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.ApplicationInsights;
using Util;
using ILogger = Logger.Contract.ILogger;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;

namespace WebAPI.Filters
{
    public class CustomActionWebApiFilter : ActionFilterAttribute
    {
        private readonly ILogger _log;
        private readonly TelemetryClient _telemetryClient;

        public CustomActionWebApiFilter()
        {
            _log = new Logger.Logger();
            _telemetryClient = new TelemetryClient();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items[context?.ActionDescriptor?.DisplayName] = Stopwatch.StartNew();
            context.HttpContext.Items["RequestData"] = context.ActionArguments.Values;

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var success = true;
            var request = context.HttpContext.Items["RequestData"] ?? "";
            var tokenId = GetTokenValue(SerializeDeSerializeHelper.Serialize(request), "TokenId\":\"");
            var affiliateId = string.Empty;

            try
            {
                affiliateId = GetTokenValue(SerializeDeSerializeHelper.Serialize(context.Result), "AffiliateId\":\"");
                if (string.IsNullOrEmpty(tokenId))
                {
                    var type = request?.GetType().ToString();
                    if (type == "WebAPI.Models.RequestModels.B2C_BookingRequest")
                    {
                        tokenId = ((WebAPI.Models.RequestModels.B2C_BookingRequest)request)?.TokenId;
                        affiliateId = ((WebAPI.Models.RequestModels.B2C_BookingRequest)request)?.AffiliateId ?? tokenId;
                    }
                    else
                    {
                        tokenId = GetTokenValue_Get(context?.HttpContext?.Request) ?? "";
                    }
                }
            }

            catch (Exception e)
            {
                // Handle the exception as needed
            }
            var ActionName = ExtractActionName(context.ActionDescriptor.DisplayName);
            

            var stopWatch = (Stopwatch)context.HttpContext.Items[context.ActionDescriptor.DisplayName];
            stopWatch.Stop();

            _log.WriteTimer(ActionName, tokenId, "Isango", stopWatch.Elapsed.ToString());
            var result = string.Empty;
            if (context.Result is ObjectResult objectResult)
            {
                result = SerializeDeSerializeHelper.Serialize(objectResult.Value);
            }
            else
            {
                if (context.Exception != null)
                {
                    success = false;
                    var problemDetails = new ProblemDetails
                    {
                        Title = context.Exception.Message,
                        Detail = context.Exception.StackTrace,
                        Status = StatusCodes.Status500InternalServerError
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
            _log.Write(SerializeDeSerializeHelper.Serialize(request),
                result, ActionName, tokenId, "Isango");

            try
            {
                _telemetryClient.TrackEvent(context.ActionDescriptor.DisplayName, new Dictionary<string, string>()
                {
                    {"Referrer", context.HttpContext.Request.Headers["Referer"].ToString()},
                    {"Request", SerializeDeSerializeHelper.Serialize(request) },
                    {"Response", result},
                    {"Method", ActionName},
                    {"AffiliateId", affiliateId},
                    {"Token", tokenId}
                });
                //_telemetryClient.TrackRequest(context.ActionDescriptor.DisplayName, DateTimeOffset.Now, stopWatch.Elapsed, context.HttpContext.Response.StatusCode.ToString(), success);
            }
            catch (Exception ex)
            {
                try
                {
                    _telemetryClient.TrackEvent(context.ActionDescriptor.DisplayName, new Dictionary<string, string>()
                    {
                        {"Request", SerializeDeSerializeHelper.Serialize(request) },
                        {"Response", result},
                        {"Method", ActionName},
                        {"AffiliateId", affiliateId},
                        {"Token", tokenId}
                    });
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        private string ExtractActionName(string actionDescriptorDisplayName)
        {
            string[] parts = actionDescriptorDisplayName.Split('.');

            if (parts.Length >= 2)
            {
                // Extract the method name from the second part
                string[] methodNameParts = parts[3].Split(' ');
                return methodNameParts[0];
            }

            return actionDescriptorDisplayName; // Return full path if the format is unexpected
        }

        public string GetTokenValue(string value, string startValue)
        {
            return value.Contains(startValue) ? value.Split(new[] { startValue }, StringSplitOptions.None)[1]
                .Split('"')[0]
                .Trim() : string.Empty;
        }

        private string GetTokenValue_Get(HttpRequest request)
        {
            var query = HttpUtility.ParseQueryString(request?.QueryString.Value);
            if (query != null)
            {
                return query["tokenId"];
            }
            return null;
        }
    }
}
