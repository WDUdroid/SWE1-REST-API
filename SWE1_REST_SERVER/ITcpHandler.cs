using System;
using System.IO;
using System.Net.Sockets;

namespace SWE1_REST_SERVER
{
    public interface ITcpHandler : IDisposable
    {
        Stream GetStream();
        void AcceptTcpClient();
        int DataAvailable();
    }
}