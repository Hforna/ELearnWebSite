using Course.Domain.Services.Rest;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Rest
{
    public class LinkService : ILinkService
    {
        private readonly IUrlHelper _urlHelper;

        public LinkService(IUrlHelper urlHelper) => _urlHelper = urlHelper;

        public string GenerateResourceLink(string routeName, object routeValues) => _urlHelper.Link(routeName, routeValues);

    }
}
