using Course.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Course.Api.Filters
{
    public class FilterException : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is BaseException based)
            {
                context.HttpContext.Response.StatusCode = (int)based.GetStatusCode();
                context.Result = new BadRequestObjectResult(new JsonErrorResponse(based.GetMessage()));
            } else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
