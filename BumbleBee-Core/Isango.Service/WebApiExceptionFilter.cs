
using System.Net;

namespace Isango.Service
{
    public class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public WebApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public WebApiException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
