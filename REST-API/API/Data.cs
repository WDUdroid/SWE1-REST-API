using System;
using System.Collections.Generic;
using System.Text;

namespace REST_API.API
{
    class Data
    {
        private List<string> messagesData = new List<string>();
        private ResponseHandler _responseHandle;

        public Data(ResponseHandler responseHandle)
        {
            _responseHandle = responseHandle;
        }

        public void NewMessage(string message)
        {
            Console.WriteLine(">>Sent request checks out");

            _responseHandle.StatusCode = "200 OK";
            messagesData.Add(message);
            _responseHandle.MessageID = messagesData.Count;
            _responseHandle.Payload = message;

            Console.WriteLine(">>Responding with 200 OK");
        }
        public void UpdateMessage(int id, string message)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent MessageID out of Range");

                _responseHandle.StatusCode = "412 Precondition Failed";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = "";

                Console.WriteLine(">>Responding with 412 Precondition Failed");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                _responseHandle.StatusCode = "200 OK";
                messagesData[id - 1] = message;
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = message;

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        public void ListMessages()
        {
            Console.WriteLine(">>Sent request checks out");

            _responseHandle.StatusCode = "200 OK";
            _responseHandle.Payload = "";
            for (int i = 0; i < messagesData.Count; i++)
            {
                _responseHandle.Payload += i+1;
                _responseHandle.Payload += ": " + messagesData[i];
                _responseHandle.Payload += "\r\n";
            }

            _responseHandle.MessageID = -1;

            Console.WriteLine(">>Responding with 200 OK");
        }

        public void ListSingleMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                _responseHandle.StatusCode = "404 Not Found";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                _responseHandle.StatusCode = "200 OK";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = messagesData[id - 1];

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        public void RemoveMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                _responseHandle.StatusCode = "404 Not Found";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                _responseHandle.StatusCode = "200 OK";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = "";
                messagesData[id-1] = "";

                Console.WriteLine(">>Responding with 200 OK");
            }
        }
    }
}
