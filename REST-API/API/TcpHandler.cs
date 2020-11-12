using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace REST_API.API
{
    public class TcpHandler : ITcpHandler
    {
        private readonly System.Net.Sockets.TcpListener _server;
        private System.Net.Sockets.TcpClient _client;

        public TcpHandler()
        {
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 8000);
        }

        public TcpHandler(TcpClient client, IPAddress domain)
        {
            try
            {
                if (client != null && domain != null)
                {
                    _server = new System.Net.Sockets.TcpListener(domain, 8000);
                    _server.Start();
                    _client = client;
                    _client.Connect(domain, 8000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AcceptTcpClient()
        {
            _client = _server.AcceptTcpClient();
        }

        public string GetHttpContent()
        {
            var stream = _client.GetStream();
            var receivedData = "";

            while (stream.DataAvailable)
            {
                Byte[] bytes = new Byte[4096];
                int i = stream.Read(bytes, 0, bytes.Length);
                receivedData += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            }

            return receivedData;
        }

        public void Start()
        {
            _server.Start(5);
        }
        public Stream GetStream() => _client.GetStream();
        public void Close()
        {
            _client.Close();
        }
        public void Stop()
        {
            _server.Stop();
        }
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}