using System.Globalization;

namespace Payment.Api.Middlewares
{
    public class CultureInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureInfoMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var requestCulture = context.Request.Headers.AcceptLanguage;
            var acceptedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var currentCulture = new CultureInfo("en-US");

            if (string.IsNullOrEmpty(requestCulture) == false && acceptedLanguages.Any(d => d.Equals(requestCulture)))
                currentCulture = new CultureInfo(requestCulture);
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentCulture;

            await _next(context);
        }
    }
}
