using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SWE1_REST_SERVER
{
    class RestServer
    {
        static readonly SemaphoreSlim concurrentConnections = new SemaphoreSlim(1);
        private static List<string> messagesData = new List<string>();

        static Task Main(string[] args)
        {
            Console.WriteLine("__________STARTING REST-SERVER__________");

            TcpHandler tcpHandler = null;
            var tasks = new List<Task>();

            try
            {
                tcpHandler = new TcpHandler();

                Console.WriteLine(">>REST-Server startup successful");
                Console.WriteLine("...");
                Console.WriteLine("________________________________________");
                Console.WriteLine();
                Console.WriteLine("____SWITCHING TO CLIENT SERVICE AREA____");

                while (true)
                {
                    concurrentConnections.Wait();
                    tasks.Add(Task.Run(() => ClientReception(tcpHandler)));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                tcpHandler?.Stop();
                Task.WaitAll(tasks.ToArray());
            }
        }

        private static void ClientReception(TcpHandler tcpHandler)
        {
            WebHandler webHandler = new WebHandler(tcpHandler);
            var content = webHandler.GetHttpContent();

            Console.WriteLine("\n\n----------RECEIVED HTTP-REQUEST----------");
            Console.WriteLine(content);
            Console.WriteLine("--------RECEIVED HTTP-REQUEST END--------\n");

            webHandler.WorkHttpRequest(content, messagesData);
            webHandler.SendHttpContent();
            tcpHandler.CloseClient();

            concurrentConnections.Release();
            Console.WriteLine(">>Client finished\n\n\n\n\n");
        }
    }
}
