using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Progress.Domain.Exceptions;

namespace Progress.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is BaseException based)
            {
                context.Result = context.Result = new BadRequestObjectResult(new RestException(based.GetMessage(), System.Net.HttpStatusCode.InternalServerError));
                context.HttpContext.Response.StatusCode = (int)based.GetStatusCode();
            }
        }
    }
}
