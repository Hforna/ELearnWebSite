using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public abstract class BaseResponse
    {
        public Dictionary<string, Link> _links { get; set; } = new Dictionary<string, Link>();

        public void AddLink(string resource, string href, string method)
        {
            _links[resource] = new Link() { Href = href, Method = method };
        }
    }
}
