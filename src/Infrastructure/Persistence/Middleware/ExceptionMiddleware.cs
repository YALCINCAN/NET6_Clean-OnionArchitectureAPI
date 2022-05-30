using Application.Exceptions;
using Application.Wrappers.Concrete;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Net;

namespace Persistence.Middleware
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            if (e.InnerException is ApiException || e.GetType() == typeof(ApiException))
            {
                var ex = e.InnerException != null ? (ApiException)e.InnerException : (ApiException)e;
                httpContext.Response.StatusCode = ex.StatusCode;
                var apierror = JsonConvert.SerializeObject(new ErrorResponse(ex.StatusCode, ex.Errors), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return httpContext.Response.WriteAsync(apierror);
            }

            List<string> exceptions = new List<string>();

            if (e.InnerException != null)
            {
                exceptions.Add(e.InnerException.ToString());
                if (e.InnerException.Message != null)
                {
                    exceptions.Add(e.InnerException.Message);
                }
                else if (e.InnerException.InnerException.Message != null)
                {
                    exceptions.Add(e.InnerException.InnerException.Message);
                }
            }
            else if (e.Message != null)
            {
                exceptions.Add(e.Message);
            }
            var errorlogDetail = new
            {
                Errors = exceptions,
            };
            var serializederror = JsonConvert.SerializeObject(errorlogDetail, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            Log.Error(serializederror);
            var error = JsonConvert.SerializeObject(new ErrorResponse(httpContext.Response.StatusCode, message), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return httpContext.Response.WriteAsync(error);
        }
    }
}