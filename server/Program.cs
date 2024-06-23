using Greeting;
using Grpc.Core;
using Sum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    Services = { 
                        GreetingService.BindService(new GreetingServiceImp()),
                        SumService.BindService(new SumServiceImp()),
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine($"Server listening on the port : {Port}");
                Console.ReadKey();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Server failed to start {ex.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}
