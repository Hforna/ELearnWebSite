using Course.Domain.Services.Token;

namespace Course.Api.Filters
{
    public class TokenReceptor : ITokenReceptor
    {
        private readonly IHttpContextAccessor _httpContext;

        public TokenReceptor(IHttpContextAccessor httpContext) => _httpContext = httpContext;

        public string? GetToken()
        {
            var httpContext = _httpContext.HttpContext!;

            if (httpContext is null)
                return "";

            var token = httpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(token))
                throw new BadHttpRequestException("No token");

            return token["Bearer ".Length..].Trim();
        }
    }
}
