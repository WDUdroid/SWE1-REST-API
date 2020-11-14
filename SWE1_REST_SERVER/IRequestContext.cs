using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_SERVER
{
    public interface IRequestContext
    {
        string StatusCode { get; set; }
        string Payload { get; set; }
        int MessageID { get; set; }

        void RequestFulfill();
    }
}
