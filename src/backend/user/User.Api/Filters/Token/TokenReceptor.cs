namespace User.Api.Filters.Token
{
    public class TokenReceptor : ITokenReceptor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenReceptor(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor;

        public string GetToken()
        {
            var token = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(token))
                throw new BadHttpRequestException("No token");
            return token["Bearer".Length..].Trim();
        }
    }
}
