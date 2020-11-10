using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using REST_API.API;

namespace REST_API
{
    class RestAPI
    {
        static SemaphoreSlim concurrentConnections = new SemaphoreSlim(1);
        static Task Main(string[] args)
        {
            Console.WriteLine("__________STARTING REST-SERVER__________");

            TcpListener server = null;
            var tasks = new List<Task>();

            try
            {
                server = new TcpListener(IPAddress.Loopback, 8000);
                server.Start();

                Console.WriteLine();
                Console.WriteLine(">>REST-Server startup successful");
                Console.WriteLine("...");
                Console.WriteLine("________________________________________");
                Console.WriteLine();
                Console.WriteLine("____SWITCHING TO CLIENT SERVICE AREA____");

                while (true)
                {
                    concurrentConnections.Wait();
                    tasks.Add(Task.Run(() => ClientReception(server)));

                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                server?.Stop();
                Task.WaitAll(tasks.ToArray());
            }
        }

        private static void ClientReception(TcpListener server)
        {
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>Waiting for a client<<<<<<<<<<");
            Console.WriteLine("...");
            try
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine(">>Servicing client");
                Console.WriteLine("...");
                var com = new ComHandler(client);

                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            concurrentConnections.Release();
            Console.WriteLine(">>Client finished\n\n\n\n\n");
        }
    }
}
