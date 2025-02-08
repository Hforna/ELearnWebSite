using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Rest
{
    public interface ILinkService
    {
        public string GenerateResourceLink(string routeName, object routeValues);
    }
}
