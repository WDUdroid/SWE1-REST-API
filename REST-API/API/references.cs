using System;
using System.Collections.Generic;
using System.Text;

namespace REST_API.API
{
    public enum RESTfulMethods
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class Constants
    {
        public const string ResponseHttpVersion = "HTTP/1.1";
        public const string ServerName = "RESTful-Server";
    }
}
