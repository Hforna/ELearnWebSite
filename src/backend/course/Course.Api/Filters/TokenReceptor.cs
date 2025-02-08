using Course.Domain.Services.Token;

namespace Course.Api.Filters
{
    public class TokenReceptor : ITokenReceptor
    {
        private readonly IHttpContextAccessor _httpContext;

        public TokenReceptor(IHttpContextAccessor httpContext) => _httpContext = httpContext;

        public string? GetToken()
        {
            var token = _httpContext.HttpContext!.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(token))
                return null;

            return token["Bearer ".Length..].Trim();
        }
    }
}
