using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;

namespace WebAPI.Models
{
    public class HttpActionResult<T> : IActionResult
    {
        private readonly T _apiResponseObject;
        private readonly HttpRequest _request;

        public HttpActionResult(T apiResponseObject, HttpRequest request)
        {
            _request = request;
            _apiResponseObject = apiResponseObject;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(_apiResponseObject), System.Text.Encoding.UTF8, "application/json"),
            };

            var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
            if (responseFeature != null)
            {
                responseFeature.ReasonPhrase = "OK";
                responseFeature.StatusCode = (int)HttpStatusCode.OK;
            }

            context.HttpContext.Response.Headers.Clear();
            foreach (var header in response.Headers)
            {
                context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in response.Content.Headers)
            {
                context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
            }

            await context.HttpContext.Response.Body.WriteAsync(await response.Content.ReadAsByteArrayAsync());
        }
    }
}
