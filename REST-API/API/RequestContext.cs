using System;
using System.Collections.Generic;
using System.Linq;
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

        private ResponseHandler _responseHandle;
        private Data _data;

        public RequestContext(string receivedData, ResponseHandler responseHandle, Data data)
        {
            _responseHandle = responseHandle;
            _data = data;

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

            int posAddOn = 2;

            HttpBody = BodyExists == true ? dataSnippets[contentLengthPos+posAddOn] : "";

            while (HttpBody.Length != Int32.Parse(bodyDataFilter[1]))
            {
                posAddOn = posAddOn + 1;
                HttpBody += "\r\n";
                HttpBody += dataSnippets[contentLengthPos + posAddOn];
            }
            
            RequestFulfill();
        }

        public void RequestFulfill()
        {
            string[] httpRequestSnippets = HttpRequest.Split("/");

            if (RequestMethod == "GET" && httpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("Detected GET-Request\n");

                if (httpRequestSnippets.Length == 2)
                {
                    _data.ListMessages();
                }
                else if(httpRequestSnippets.Length == 3)
                {
                    try
                    {
                        _data.ListSingleMessage(int.Parse(httpRequestSnippets[2]));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                        Console.WriteLine(e);
                        Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                        _responseHandle.StatusCode = "400 Bad Request";
                        _responseHandle.MessageID = -1;
                        _responseHandle.Payload = "";

                        Console.WriteLine(">>Responding with 400 Bad Request\n");
                    };
                }
            }

            else if (RequestMethod == "POST" && httpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected POST-Request");
                _data.NewMessage(HttpBody);
            }

            else if (RequestMethod == "PUT" && httpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected PUT-Request");
                try
                {
                    _data.UpdateMessage(int.Parse(httpRequestSnippets[2]), HttpBody);
                }
                catch (Exception e)
                {
                    Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                    Console.WriteLine(e);
                    Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                    _responseHandle.StatusCode = "400 Bad Request";
                    _responseHandle.MessageID = -1;
                    _responseHandle.Payload = "";

                    Console.WriteLine(">>Responding with 400 Bad Request");
                };
            }
            
            else if (RequestMethod == "DELETE" && httpRequestSnippets[1] == "messages")
            {
                Console.WriteLine("\nDetected DELETE-Request");
                try
                {
                    _data.RemoveMessage(int.Parse(httpRequestSnippets[2]));
                }
                catch (Exception e)
                {
                    Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                    Console.WriteLine(e);
                    Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                    _responseHandle.StatusCode = "400 Bad Request";
                    _responseHandle.MessageID = -1;
                    _responseHandle.Payload = "";

                    Console.WriteLine(">>Responding with 400 Bad Request");
                };
            }

            else
            {
                _responseHandle.StatusCode = "400 Bad Request";
                _responseHandle.MessageID = -1;
                _responseHandle.Payload = "";
                Console.WriteLine(">>Responding with 400 Bad Request");
            }

            _responseHandle.SendResponse();
        }
    }
}
