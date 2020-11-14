using System;
using System.IO;
using System.Net.Sockets;

namespace SWE1_REST_SERVER
{
    internal interface ITcpHandler : IDisposable
    {
        NetworkStream GetStream();
        void AcceptTcpClient();
    }
}