using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using REST_API.API;

namespace REST_API
{
    class ResponseHandler
    {
        public string StatusCode;
        public int MessageID;
        public string Payload = String.Empty;
        public string Response;


        public void SendResponse()
        {
            Response = Constants.ResponseHttpVersion + " " + StatusCode + "\r\n" 
                             + "Server: " + Constants.ServerName +"\r\n";

            if (Payload != "")
            {
                Response += "Content-Type: text/plain\r\n";

                if (MessageID < 0)
                {
                    Response += "Content-Length: " + Payload.Length + "\r\n\r\n" 
                                + Payload;
                }
                else
                {
                    var mlength = MessageID.ToString().Length + 1 + Payload.Length;
                    Response += "Content-Length: " + mlength + "\r\n\r\n" + MessageID.ToString() + " " + Payload;
                }
            }
        }
    }
}
