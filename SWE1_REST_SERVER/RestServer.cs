using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


// REST-SERVER v2.0
// Autor: David Kaiblinger
//
// Version 2 was created with testing in mind. while "Cowboy Coding" v1.0 I did not thing about
// implementing testing procedures, which obviously made testing really hard.
//
// After talking to Markus Altenhofer I was inspired to rework the whole project.
// 

namespace SWE1_REST_SERVER
{
    class RestServer
    {
        // Setting up multiple Clients, for demonstration purposes allow only one thread at a time,
        // which alleviates the need for mutex
        static readonly SemaphoreSlim concurrentConnections = new SemaphoreSlim(1);

        // messageData stores all messages, everything is in-memory, meaning there is no file-handling
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

        // ClientReception is like a lobby in a hotel. Here every client checks in
        // and every client checks out
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
