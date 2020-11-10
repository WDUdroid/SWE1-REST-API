using System;
using System.Collections.Generic;
using System.Text;

namespace REST_API.API
{
    class RequestContext
    {
        private bool BodyExists;
        private Dictionary<string, string> headerKeyValue;

        public string HttpBody { get; set; }

        public string HttpVersion { get; set; }

        public string HttpRequest { get; set; }

        public string RequestMethod { get; set; }

        public RequestContext(string receivedData)
        {
            
            string[] dataSnippets = receivedData.Split("\r\n");

            string[] headerDataFilter = dataSnippets[0].Split(" ");

            RESTfulMethods methodCompare;
            if (Enum.TryParse(headerDataFilter[0], out methodCompare))
            {
                RequestMethod = headerDataFilter[0];
            }
            else
            {
                Console.WriteLine("\n>>Received HTTP-Request does not apply\n");

            }

            HttpRequest = headerDataFilter[1];
            HttpVersion = headerDataFilter[2];

            int contentLengthPos = 0;

            headerKeyValue = new Dictionary<string, string>();

            for (int i = 0; i < dataSnippets.Length; i++)
            {
                string[] tmpKeyValue = dataSnippets[i].Split(": ");
                if (tmpKeyValue.Length > 1)
                {
                    headerKeyValue.Add(tmpKeyValue[0], tmpKeyValue[1]);
                    contentLengthPos = tmpKeyValue[0] == "Content-Length" ? i : 0;
                }
            }

            string[] bodyDataFilter = dataSnippets[contentLengthPos].Split(": ");

            BodyExists = bodyDataFilter[0] == "Content-Length";

            HttpBody = BodyExists == true ? dataSnippets[contentLengthPos+2] : "";

            RequestFulfill();
        }

        public void RequestFulfill()
        {
            string[] HttpRequestSnippets = HttpRequest.Split("/");

            if (RequestMethod == "GET" && HttpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("Detected GET-Request\n");

                if (HttpRequestSnippets.Length == 2)
                {
                    Data.ListMessages();
                }
                else if(HttpRequestSnippets.Length == 3)
                {
                    try
                    {
                        Data.ListSingleMessage(int.Parse(HttpRequestSnippets[2]));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                        Console.WriteLine(e);
                        Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                        ResponseHandler.StatusCode = "400 Bad Request";
                        ResponseHandler.MessageID = -1;
                        ResponseHandler.Payload = "";

                        Console.WriteLine(">>Responding with 400 Bad Request\n");
                    };
                }
            }

            else if (RequestMethod == "POST" && HttpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected POST-Request");
                Data.newMessage(HttpBody);
            }

            else if (RequestMethod == "PUT" && HttpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected PUT-Request");
                try
                {
                    Data.UpdateMessage(int.Parse(HttpRequestSnippets[2]), HttpBody);
                }
                catch (Exception e)
                {
                    Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                    Console.WriteLine(e);
                    Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                    ResponseHandler.StatusCode = "400 Bad Request";
                    ResponseHandler.MessageID = -1;
                    ResponseHandler.Payload = "";

                    Console.WriteLine(">>Responding with 400 Bad Request");
                };
            }
            
            else if (RequestMethod == "DELETE" && HttpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected DELETE-Request");
                try
                {
                    Data.RemoveMessage(int.Parse(HttpRequestSnippets[2]));
                }
                catch (Exception e)
                {
                    Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                    Console.WriteLine(e);
                    Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                    ResponseHandler.StatusCode = "400 Bad Request";
                    ResponseHandler.MessageID = -1;
                    ResponseHandler.Payload = "";

                    Console.WriteLine(">>Responding with 400 Bad Request");
                };
            }

            else
            {
                ResponseHandler.StatusCode = "400 Bad Request";
                ResponseHandler.MessageID = -1;
                ResponseHandler.Payload = "";
                Console.WriteLine(">>Responding with 400 Bad Request");
            }

            ResponseHandler.SendResponse();
        }
    }
}
