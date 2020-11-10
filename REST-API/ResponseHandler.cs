using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using REST_API.API;

namespace REST_API
{
    class ResponseHandler
    {
        public static string StatusCode;
        public static int MessageID;
        public static string Payload = String.Empty;
        public static string Response;


        public static void SendResponse()
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
