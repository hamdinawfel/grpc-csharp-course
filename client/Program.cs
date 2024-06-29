using Greeting;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace client
{
    internal class Program
    {
        const string target = "127.0.0.1:50051";
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if(task.Status  == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successffuly");
                }
            });

            
            var client = new GreetingService.GreetingServiceClient(channel);

            //UNARY
            var greeting = new Greeting.Greeting()
            {
                FirstName = "Nawfel",
                LastName = "Hamdi"
            };

            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);

            Console.WriteLine("Start UNARY GRPC test result");
            Console.WriteLine(response.Result);
            Console.WriteLine("End UNARY GRPC test result");

            //SERVER STREAM
            var requestToServerStream = new GreetingManyTimesRequest() { Greeting = greeting };
            var responseFromServerStream = client.GreetManyTimes(requestToServerStream);

            Console.WriteLine("Start SERVER STREAMING GRPC test result");

            while (await responseFromServerStream.ResponseStream.MoveNext())
            {
                await Console.Out.WriteLineAsync(responseFromServerStream.ResponseStream.Current.Result);
                await Task.Delay(500);
            }
            Console.WriteLine("End SERVER STREAMING GRPC test result");

            //CLIENT STREAM
            Console.WriteLine("Start CLIENT STREAMING GRPC test result");

            var clientStremRequest = new LongGreetingRequest { Greeting = greeting };
            var clientStream = client.LongGreet();
            foreach (var i in Enumerable.Range(0, 10))
            {
                await clientStream.RequestStream.WriteAsync(clientStremRequest);
                Console.WriteLine($"Sending req : {i} ...");
                await Task.Delay(1000);
            }

            await clientStream.RequestStream.CompleteAsync();
            var responseFormServerToCientStremRequest = await clientStream.ResponseAsync;

            Console.WriteLine(responseFormServerToCientStremRequest.Result);

            Console.WriteLine("End CLIENT STREAMING GRPC test result");

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
