using Greeting;
using Grpc.Core;
using System;
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

            Console.WriteLine(response.Result);


            //SERVER STREAM
            var requestToServerStream = new GreetingManyTimesRequest() { Greeting = greeting };
            var responseFromServerStream = client.GreetManyTimes(requestToServerStream);

            while (await responseFromServerStream.ResponseStream.MoveNext())
            {
                await Console.Out.WriteLineAsync(responseFromServerStream.ResponseStream.Current.Result);
                await Task.Delay(500);
            }

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
