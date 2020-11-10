using System;
using System.IO;
using System.Net.Sockets;

namespace REST_API.API
{
    class ComHandler
    {
        private TcpClient _client;
        private RequestContext _clientRequest;

        public ComHandler(TcpClient client)
        {
            Console.WriteLine(">>Preparing ComHandler");
            _client = client;
            RecvFromClient();
        }

        private void RecvFromClient()
        {
            Console.WriteLine(">>Starting GetStream");
            var stream = _client.GetStream();
            var recievedData = "";

            while (stream.DataAvailable)
            {
                Byte[] bytes = new Byte[4096];
                int i = stream.Read(bytes, 0, bytes.Length);
                recievedData += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            }

            Console.WriteLine("\n\n----------RECEIVED HTTP-REQUEST----------");
            Console.WriteLine(recievedData);
            Console.WriteLine("--------RECEIVED HTTP-REQUEST END--------\n");

            _clientRequest = new RequestContext(recievedData);

            using var writer = new StreamWriter(_client.GetStream()) {AutoFlush = true};
            writer.WriteLine(ResponseHandler.Response);
        }
    }
}
