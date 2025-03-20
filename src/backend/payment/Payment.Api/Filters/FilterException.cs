using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Payment.Domain.Exceptions;
using System.Net;

namespace Payment.Api.Filters
{
    public class FilterException : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is BaseException based)
            {
                context.HttpContext.Response.StatusCode = (int)based.GetStatusCode();
                context.Result = new BadRequestObjectResult(new JsonErrorResponse(based.GetMessage()));
            }
        }
    }
}
