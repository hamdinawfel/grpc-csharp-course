using Greeting;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace client
{
    internal class Program
    {
        const string target = "127.0.0.1:50051";
        static void Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            channel.ConnectAsync().ContinueWith((task) =>
            {
                if(task.Status  == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successffuly");
                }
            });

            ///SECTION 2
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting.Greeting()
            {
                FirstName = "Nawfel",
                LastName = "Hamdi"
            };

            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);

            Console.WriteLine(response.Result);


            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
