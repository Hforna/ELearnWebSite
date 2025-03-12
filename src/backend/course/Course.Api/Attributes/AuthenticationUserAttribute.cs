using Course.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace Course.Api.Attributes
{
    public class AuthenticationUserAttribute : TypeFilterAttribute
    {
        public AuthenticationUserAttribute() : base(typeof(AuthorizationUser))
        {
        }
    }
}
