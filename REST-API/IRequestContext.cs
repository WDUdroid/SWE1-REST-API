using System;
using System.Collections.Generic;
using System.Text;
using REST_API.API;

namespace REST_API
{
    interface IRequestContext
    {
        public string HttpBody { get; set; }

        public string HttpVersion { get; set; }

        public string HttpRequest { get; set; }

        public string RequestMethod { get; set; }
    }
}
