using System;
using System.IO;
using System.Net.Sockets;

namespace REST_API.API
{
    class ComHandler
    {
        private TcpHandler _client;
        private RequestContext _clientRequest;
        private ResponseHandler _responseHandle;
        private Data _data;

        public ComHandler(TcpHandler client, ResponseHandler responseHandle, Data data)
        {

            _responseHandle = responseHandle;
            _data = data;
            _client = client;

            Console.WriteLine(">>Preparing ComHandler");

            Console.WriteLine(">>Starting GetStream");
            var receivedData = _client.GetHttpContent();

            Console.WriteLine("\n\n----------RECEIVED HTTP-REQUEST----------");
            Console.WriteLine(receivedData);
            Console.WriteLine("--------RECEIVED HTTP-REQUEST END--------\n");

            _clientRequest = new RequestContext(receivedData, _responseHandle, _data);

            using var writer = new StreamWriter(_client.GetStream()) {AutoFlush = true};
            writer.WriteLine(responseHandle.Response);
        }
    }
}
