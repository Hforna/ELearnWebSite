using Course.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace Course.Api.Middlewares
{
    internal sealed class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (System.Exception ex)
            {
                HttpStatusCode statusCode = ex switch
                {
                    RequestException => HttpStatusCode.BadRequest,
                    UnauthorizedException => HttpStatusCode.Unauthorized,
                    NotFoundException => HttpStatusCode.NotFound,
                    _ => HttpStatusCode.InternalServerError
                };

                var message = ex.Message;
                if (ex is BaseException be)
                {
                    var sb = new StringBuilder();
                    sb.AppendJoin(',', be.GetMessage());
                    message = sb.ToString();
                }

                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = (int)statusCode,
                    Title = "An error occured",
                    Type = ex.GetType().Name,
                    Detail = message
                });
            }
        }
    }
}
