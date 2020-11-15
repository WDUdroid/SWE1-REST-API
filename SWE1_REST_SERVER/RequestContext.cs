using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SWE1_REST_SERVER
{
    public class RequestContext : IRequestContext
    {
        private bool BodyExists;
        private Dictionary<string, string> HeaderKeyValue;

        public string HttpBody { get; set; }
        public string HttpVersion { get; set; }
        public string HttpRequest { get; set; }
        public string RequestMethod { get; set; }
        public string StatusCode { get; set; }
        public string Payload { get; set; }
        public int MessageID { get; set; }

        private List<string> messagesData = new List<string>();

        // for testing purposes
        public RequestContext(List<string> messagesData)
        {
            this.messagesData = messagesData;
        }

        // runs in normal operation
        public RequestContext(string receivedData, List<string> messagesData)
        {
            // if receivedData does not resemble a HttpRequest an exception is thrown.
            try
            {
                this.messagesData = messagesData;
                string[] dataSnippets = receivedData.Split("\r\n");

                string[] headerDataFilter = dataSnippets[0].Split(" ");

                // Splitting header data and saving it
                RequestMethod = headerDataFilter[0];
                HttpRequest = headerDataFilter[1];
                HttpVersion = headerDataFilter[2];

                int contentLengthPos = 0;

                HeaderKeyValue = new Dictionary<string, string>();

                // Copies HttpRequest-content into key-value-pairs
                // and looks for "Content-Length"-Key, saving its index in contentLengthPos (Position)
                for (int i = 0; i < dataSnippets.Length; i++)
                {
                    string[] tmpKeyValue = dataSnippets[i].Split(": ");
                    if (tmpKeyValue.Length > 1)
                    {
                        HeaderKeyValue.Add(tmpKeyValue[0], tmpKeyValue[1]);
                        contentLengthPos = tmpKeyValue[0] == "Content-Length" ? i : 0;
                    }
                }

                string[] bodyDataFilter = dataSnippets[contentLengthPos].Split(": ");

                // Checks if HttpBody exists
                BodyExists = bodyDataFilter[0] == "Content-Length";

                // Sets up reading position for Body
                int posAddOn = 2;

                HttpBody = BodyExists == true ? dataSnippets[contentLengthPos + posAddOn] : "";

                if (BodyExists)
                {
                    // If body has multiple lines, concat' them to HttpBody
                    while (HttpBody.Length != Int32.Parse(bodyDataFilter[1]))
                    {
                        posAddOn = posAddOn + 1;
                        HttpBody += "\r\n";
                        HttpBody += dataSnippets[contentLengthPos + posAddOn];
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Checks which function is appropriate for specific HttpRequest
        public void RequestFulfill()
        {
            if (HttpRequest != null)
            {
                string[] httpRequestSnippets = HttpRequest.Split("/");

                if (RequestMethod == "GET" && httpRequestSnippets[1] == "messages")
                {
                    Console.WriteLine("Detected GET-Request\n");

                    if (httpRequestSnippets.Length == 2)
                    {
                        ListMessages();
                    }
                    else if (httpRequestSnippets.Length >= 3)
                    {
                        try
                        {
                            ListSingleMessage(int.Parse(httpRequestSnippets[2]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                            Console.WriteLine(e);
                            Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                            BadRequest();
                        }
                    }
                }

                else if (RequestMethod == "POST" && httpRequestSnippets[1] == "messages")
                {
                    Console.WriteLine("\nDetected POST-Request");
                    NewMessage(HttpBody);
                }

                else if (RequestMethod == "PUT" && httpRequestSnippets[1] == "messages")
                {
                    Console.WriteLine("\nDetected PUT-Request");
                    try
                    {
                        UpdateMessage(int.Parse(httpRequestSnippets[2]), HttpBody);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                        Console.WriteLine(e);
                        Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                        BadRequest();
                    }

                    ;
                }

                else if (RequestMethod == "DELETE" && httpRequestSnippets[1] == "messages")
                {
                    Console.WriteLine("\nDetected DELETE-Request");
                    try
                    {
                        RemoveMessage(int.Parse(httpRequestSnippets[2]));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!!!!!!! ERROR !!!!!!!!");
                        Console.WriteLine(e);
                        Console.WriteLine("!!!!!! ERROR END !!!!!!\n");

                        BadRequest();
                    }

                    ;
                }

                else
                {
                    BadRequest();
                }
            }
            else
            {
                BadRequest();
            }

        }

        private void BadRequest()
        {
            StatusCode = "400 Bad Request";
            MessageID = -1;
            Payload = "";
            Console.WriteLine(">>Responding with 400 Bad Request");
        }
        
        
        //////////////////////////////////////////////////////////////
        // Message Area
        // This area creates individual responses

        // Creates new message
        public void NewMessage(string message)
        {
            Console.WriteLine(">>Sent request checks out");

            StatusCode = "201 Created";
            messagesData.Add(message);
            MessageID = messagesData.Count;
            Payload = message;

            Console.WriteLine(">>Responding with 201 Created");
        }

        // Overwrites message with specific ID
        public void UpdateMessage(int id, string message)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent MessageID out of Range");

                StatusCode = "412 Precondition Failed";
                MessageID = -1;
                Payload = "";

                Console.WriteLine(">>Responding with 412 Precondition Failed");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                StatusCode = "200 OK";
                messagesData[id - 1] = message;
                MessageID = -1;
                Payload = message;

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        // Returns all messages
        public void ListMessages()
        {
            Console.WriteLine(">>Sent request checks out");

            StatusCode = "200 OK";
            Payload = "";
            for (int i = 0; i < messagesData.Count; i++)
            {
                Payload += i + 1;
                Payload += ": " + messagesData[i];
                Payload += "\r\n";
            }

            MessageID = -1;

            Console.WriteLine(">>Responding with 200 OK");
        }

        // Filters for specific ID and returns it with message
        public void ListSingleMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                StatusCode = "404 Not Found";
                MessageID = -1;
                Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                StatusCode = "200 OK";
                MessageID = -1;
                Payload = messagesData[id - 1];

                Console.WriteLine(">>Responding with 200 OK");
            }
        }

        // Overwrites message with empty string
        public void RemoveMessage(int id)
        {
            if (id > messagesData.Count)
            {
                Console.WriteLine(">>Sent request asking for non existent entry");

                StatusCode = "404 Not Found";
                MessageID = -1;
                Payload = "";

                Console.WriteLine(">>Responding with 404 Not Found");
            }
            else
            {
                Console.WriteLine(">>Sent request checks out");

                StatusCode = "200 OK";
                MessageID = -1;
                Payload = "";
                messagesData[id - 1] = "";

                Console.WriteLine(">>Responding with 200 OK");
            }
        }
    }
}
