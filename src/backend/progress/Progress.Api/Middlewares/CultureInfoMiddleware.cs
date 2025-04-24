
using System.Globalization;

namespace Progress.Api.Middlewares
{
    public class CultureInfoMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestLanguage = context.Request.Headers.AcceptLanguage;
            var acceptedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var currentCulture = new CultureInfo("en-US");

            if(string.IsNullOrEmpty(requestLanguage) == false && acceptedLanguages.Any(d => d.Equals(acceptedLanguages)))
            {
                currentCulture = new CultureInfo(requestLanguage!);
            }

            CultureInfo.CurrentUICulture = currentCulture;
            CultureInfo.CurrentCulture = currentCulture;

            await next(context);
        }
    }
}
