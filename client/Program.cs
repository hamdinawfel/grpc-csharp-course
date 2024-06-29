using Calculator;
using Greeting;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Calculator.CalculatorService;
using static Greeting.GreetingService;

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

            
            var client = new GreetingServiceClient(channel);
            var calculatorClient = new CalculatorServiceClient(channel);

            var greeting = new Greeting.Greeting()
            {
                FirstName = "Nawfel",
                LastName = "Hamdi"
            };

            //UNARY
            //Greet(client, greeting);
            //SERVER STREAM
            //await GreetingManyTimes(client, greeting);
            //CLIENT STREAM
            //await LongGreet(client, greeting);
            //BIDI STREAMING EXERCICE
            await GreetEveryone(client);

            //CLIENT STREAMING EXERCICE
            //await CalculateAvg(calculatorClient);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        private static void Greet(GreetingServiceClient client, Greeting.Greeting greeting)
        {

            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);

            Console.WriteLine("Start UNARY GRPC test result");
            Console.WriteLine(response.Result);
            Console.WriteLine("End UNARY GRPC test result");
        }
        private static async Task GreetingManyTimes(GreetingServiceClient client, Greeting.Greeting greeting)
        {
            var request = new GreetingManyTimesRequest() { Greeting = greeting };
            var responseStream = client.GreetManyTimes(request);

            Console.WriteLine("Start SERVER STREAMING GRPC test result");

            while (await responseStream.ResponseStream.MoveNext())
            {
                await Console.Out.WriteLineAsync(responseStream.ResponseStream.Current.Result);
                await Task.Delay(100);
            }
            Console.WriteLine("End SERVER STREAMING GRPC test result");
        }

        private static async Task LongGreet(GreetingServiceClient client, Greeting.Greeting greeting)
        {
            Console.WriteLine("Start CLIENT STREAMING GRPC test result");

            var request = new LongGreetingRequest { Greeting = greeting };
            var clientStreamCall = client.LongGreet();
            foreach (var i in Enumerable.Range(0, 10))
            {
                await clientStreamCall.RequestStream.WriteAsync(request);
                Console.WriteLine($"Sending req : {i} ...");
                await Task.Delay(100);
            }

            await clientStreamCall.RequestStream.CompleteAsync();
            var response = await clientStreamCall.ResponseAsync;

            Console.WriteLine(response.Result);

            Console.WriteLine("End CLIENT STREAMING GRPC test result");
        }

        private static async Task GreetEveryone(GreetingServiceClient client)
        {
            var stream = client.GreetEveryone();
            Console.WriteLine("Start bidi streming GRPC test result");

            var responseStremTask = Task.Run(async () =>
            {
                while(await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine($"client receive : ${stream.ResponseStream.Current.Result}");
                    await Task.Delay(100);
                }
            });

            Greeting.Greeting[] greetings =
            {
                new Greeting.Greeting { FirstName = "Nawfel", LastName = "Hamdi"},
                new Greeting.Greeting { FirstName = "Jgon", LastName = "Doe"},
                new Greeting.Greeting { FirstName = "Eric", LastName = "Martinez"}
            };

            foreach (var greeting in greetings)
            {
                await Console.Out.WriteLineAsync($"Sending Greeting : {greeting.ToString()}");
                await stream.RequestStream.WriteAsync(new GreetingEveryoneRequest { Greeting = greeting});
                await Task.Delay(1000);
            }

            await stream.RequestStream.CompleteAsync();

            await responseStremTask;

            Console.WriteLine("End bidi streming GRPC test result");
        }

        private static async Task CalculateAvg(CalculatorServiceClient client)
        {
            Console.WriteLine("Start CLIENT STREAMING EXERICE");


            var numbers = new List<int> { 1, 2, 3, 4 };

            var avgRequest = client.CalculateAverage();
            foreach (var number in numbers)
            {
                await avgRequest.RequestStream.WriteAsync(new AverageCalculatorRequest { Number = number });
            }
            await avgRequest.RequestStream.CompleteAsync();
            var avgResponse = await avgRequest.ResponseAsync;
            Console.WriteLine(avgResponse.Result);

            Console.WriteLine("End CLIENT STREAMING EXERICE");
        }
    }
}
