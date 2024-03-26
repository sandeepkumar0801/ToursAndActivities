using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Util;
using Isango.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var tokenId = GetValues(SerializeDeSerializeHelper.Serialize(context.ActionDescriptor.RouteValues["TokenId"]), "TokenId\":\"");
            var affiliateId = GetValues(SerializeDeSerializeHelper.Serialize(context.ActionDescriptor.RouteValues["AffiliateId"]), "AffiliateId\":\"");

            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "CustomExceptionFilter",
                MethodName = "OnException",
                Token = tokenId,
                AffiliateId = affiliateId,
                Params = $"WebAPI|{context.ActionDescriptor.DisplayName}|{context.HttpContext.Request.Path}"
            };

            _logger.LogError(context.Exception, "An error has occurred. Details: {ErrorEntity}", isangoErrorEntity);

            var error = new
            {
                Message = "An error has occurred.",
                ExceptionMessage = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                StatusCode = HttpStatusCode.InternalServerError
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        public string GetValues(string value, string startValue)
        {
            return value.Contains(startValue) ? value.Split(new[] { startValue }, StringSplitOptions.None)[1]
                .Split('"')[0]
                .Trim() : string.Empty;
        }
    }
}
