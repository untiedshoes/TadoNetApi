using System;
using System.Net;

namespace TadoNetApi.Infrastructure.Exceptions
{
    public class TadoApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public TadoApiException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}