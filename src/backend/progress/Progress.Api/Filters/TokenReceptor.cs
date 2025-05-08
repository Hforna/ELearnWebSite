using Progress.Domain.Token;

namespace Progress.Api.Filters
{
    public class TokenReceptor : ITokenReceptor
    {
        private readonly IHttpContextAccessor _httpContext;

        public TokenReceptor(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetToken()
        {
            var token = _httpContext.HttpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(token))
                return "";

            return token["Bearer ".Length..].Trim();
        }
    }
}
