using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace REST_API
{
    public class TcpHandler : ITcpListener
    {
        private readonly System.Net.Sockets.TcpListener _server;
        private System.Net.Sockets.TcpClient _client;

        public TcpHandler()
        {
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 8000);
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
            _server.Start();
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