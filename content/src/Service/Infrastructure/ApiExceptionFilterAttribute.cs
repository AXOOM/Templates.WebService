using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyVendor.MyService.Infrastructure
{
    /// <summary>
    /// Reports exceptions with appropriate HTTP status codes.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly bool _isDevelopment;
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(IWebHostEnvironment env, ILogger<ApiExceptionFilterAttribute> logger)
        {
            _isDevelopment = env.IsDevelopment();
            _logger = logger;
        }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            var (statusCode, logLevel) = GetStatusCodeAndLogLevel(context.Exception);
            var request = context.HttpContext.Request;

            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = BuildResult(context.Exception);

            _logger.Log(logLevel, context.Exception, "Responded to HTTP {0} {1} with {2} due to exception.",
                request.Method, request.GetEncodedPathAndQuery(), statusCode);

            base.OnException(context);
        }

        private static (HttpStatusCode, LogLevel) GetStatusCodeAndLogLevel(Exception exception)
        {
            switch (exception)
            {
                case AuthenticationException _:
                    return (HttpStatusCode.Unauthorized, LogLevel.Debug);
                case UnauthorizedAccessException _:
                    return (HttpStatusCode.Forbidden, LogLevel.Debug);
                case InvalidDataException _:
                    return (HttpStatusCode.BadRequest, LogLevel.Information);
                case KeyNotFoundException _:
                    return (HttpStatusCode.NotFound, LogLevel.Information);
                case InvalidOperationException _:
                    return (HttpStatusCode.Conflict, LogLevel.Warning);
                case TimeoutException _:
                    return (HttpStatusCode.RequestTimeout, LogLevel.Warning);
                default:
                    return (HttpStatusCode.InternalServerError, LogLevel.Error);
            }
        }

        private ObjectResult BuildResult(Exception exception)
            => new ObjectResult(new
            {
                message = exception.Message,
                type = exception.GetType().Name,
                stackTrace = _isDevelopment ? exception.Demystify().StackTrace : null
            });
    }
}
