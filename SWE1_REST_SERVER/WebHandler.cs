using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWE1_REST_SERVER
{
    public class WebHandler : IWebHandler
    {
        private readonly ITcpHandler _tcpHandler;

        private IRequestContext _requestContext;

        // used while normal operation
        public WebHandler(ITcpHandler tcpHandler)
        {
            Console.WriteLine();

            _tcpHandler = tcpHandler;
            _tcpHandler.AcceptTcpClient();
        }

        // for testing purposes
        public WebHandler(ITcpHandler tcpHandler, IRequestContext requestContext)
        {
            _tcpHandler = tcpHandler;
            _tcpHandler.AcceptTcpClient();

            _requestContext = requestContext;
        }

        // reads message sent by client
        // Streamreader did not work, which made testing a little harder
        // Instead of getting .DataAvailable() from the StreamReader-Object,
        // I had to check, if the TcpClient has available data.
        public string GetHttpContent()
        {
            var stream = _tcpHandler.GetStream();
            var receivedData = "";

            while (_tcpHandler.DataAvailable() != 0)
            {
                Byte[] bytes = new Byte[4096];
                int i = stream.Read(bytes, 0, bytes.Length);
                receivedData += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            }

            return receivedData;
        }

        public void WorkHttpRequest(string content, List<String> messageData)
        {
            _requestContext = new RequestContext(content, messageData);
            _requestContext.RequestFulfill();
        }

        public void SendHttpContent()
        {
            var response = "HTTP/1.1" + " " + _requestContext.StatusCode + "\r\n"
                       + "Server: " + "RESTful-Server" + "\r\n";

            if (_requestContext.Payload != "")
            {
                response += "Content-Type: text/plain\r\n";

                if (_requestContext.MessageID < 0)
                {
                    response += "Content-Length: " + _requestContext.Payload.Length + "\r\n\r\n"
                                + _requestContext.Payload;
                }
                else
                {
                    var mlength = _requestContext.MessageID.ToString().Length + 1 + _requestContext.Payload.Length;
                    response += "Content-Length: " + mlength + "\r\n\r\n" + _requestContext.MessageID.ToString() + " " + _requestContext.Payload;
                }
            }

            Console.WriteLine(response);
            using StreamWriter writer = new StreamWriter(_tcpHandler.GetStream()) { AutoFlush = true };
            writer.WriteLine(response);
        }
    }
}
