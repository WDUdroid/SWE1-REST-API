using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REST_API.API
{
    public class WebContentReader
    {
        private ITcpHandler tcpHandler;

        public WebContentReader(ITcpHandler tcpHandler)
        {
            this.tcpHandler = tcpHandler;
        }

        public string GetHttpContent()
        {
            using StreamReader reader = new StreamReader(tcpHandler.GetStream());
            return reader.ReadToEnd();
        }
    }
}
