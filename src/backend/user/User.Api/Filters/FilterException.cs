using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using User.Api.Excpetions;

namespace User.Api.Filters
{
    public class FilterException : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is BaseProjectException baseProjectException)
            {
                context.HttpContext.Response.StatusCode = baseProjectException.GetStatusCode();
                context.Result = new BadRequestObjectResult(baseProjectException.GetErrorMessage());
            }
        }
    }
}
