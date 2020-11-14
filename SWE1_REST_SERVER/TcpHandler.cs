using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SWE1_REST_SERVER
{
    class TcpHandler : ITcpHandler
    {
        private TcpListener _server;
        public TcpClient _client;

        public TcpHandler()
        {
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 8000);
            Console.WriteLine(_server.ToString());
            _server.Start(5);
        }

        public void AcceptTcpClient()
        {
            _client = _server.AcceptTcpClient();
        }

        public NetworkStream GetStream() => _client.GetStream();
        public void CloseClient() => _client.Close();

        public void Stop() => _server.Stop();

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
