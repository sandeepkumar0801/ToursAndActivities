using Isango.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Util
{
    public class HttpResponseException : Exception
    {
        public HttpResponseMessage Response { get; }
        public IsangoErrorEntity ErrorEntity { get; }

        public HttpResponseException(HttpResponseMessage httpResponse, IsangoErrorEntity errorEntity)
        {
            Response = httpResponse;
            ErrorEntity = errorEntity;
        }

        public HttpResponseException(HttpResponseMessage response)
        {
            Response = response;
        }
    }
}
