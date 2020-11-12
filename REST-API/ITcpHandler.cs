using System;
using System.IO;

namespace REST_API
{
    public interface ITcpHandler : IDisposable
    {
        Stream GetStream();
        string GetHttpContent();
        public void AcceptTcpClient();
    }
}
