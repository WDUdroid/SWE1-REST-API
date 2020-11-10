using System;
using System.Collections.Generic;
using System.Text;

namespace REST_API.API
{
    class Data
    {
        private static List<string> messagesData = new List<string>();

        public static void newMessage(string message)
        {
            Console.WriteLine(">>Sent request checks out");

            ResponseHandler.StatusCode = "200 OK";
            messagesData.Add(message);
            ResponseHandler.MessageID = messagesData.Count;
            ResponseHandler.Payload = message;

            Console.WriteLine(">>Responding with 200 OK");
        }
        public static void UpdateMessage(int id, string message)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent MessageID out of Range");

                ResponseHandler.StatusCode = "412 Precondition Failed";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = "";

                Console.WriteLine(">>Responding with 412 Precondition Failed");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                ResponseHandler.StatusCode = "200 OK";
                messagesData[id - 1] = message;
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = message;

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        public static void ListMessages()
        {
            Console.WriteLine(">>Sent request checks out");

            ResponseHandler.StatusCode = "200 OK";
            ResponseHandler.Payload = "";
            for (int i = 0; i < messagesData.Count; i++)
            {

                ResponseHandler.Payload += messagesData[i];
                if (i + 1 != messagesData.Count)
                {
                    ResponseHandler.Payload += ", ";
                }
            }

            ResponseHandler.MessageID = -1;

            Console.WriteLine(">>Responding with 200 OK");
        }

        public static void ListSingleMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                ResponseHandler.StatusCode = "404 Not Found";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                ResponseHandler.StatusCode = "200 OK";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = messagesData[id - 1];

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        public static void RemoveMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                ResponseHandler.StatusCode = "404 Not Found";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                ResponseHandler.StatusCode = "200 OK";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = "";
                messagesData.RemoveAt(id-1);

                Console.WriteLine(">>Responding with 200 OK");
            }
        }
    }
}
