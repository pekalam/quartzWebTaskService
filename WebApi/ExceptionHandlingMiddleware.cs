using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi
{
    public class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public WebApiException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public WebApiException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (WebApiException ex)
            {
                await HandleWebApiException(ex, context);
            }
        }

        private async Task HandleWebApiException(WebApiException ex, HttpContext context)
        {
            context.Response.StatusCode = (int) ex.StatusCode;
            await context.Response.WriteAsync($"{ex.StatusCode} {ex.Message}");
        }
    }
}
