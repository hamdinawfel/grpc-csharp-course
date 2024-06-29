using Calculator;
using Greeting;
using Grpc.Core;
using System;
using System.Collections.Generic;
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
                await Task.Delay(100);
            }
            Console.WriteLine("End SERVER STREAMING GRPC test result");

            //CLIENT STREAM
            Console.WriteLine("Start CLIENT STREAMING GRPC test result");

            var clientStremRequest = new LongGreetingRequest { Greeting = greeting };
            var clientStreamReq = client.LongGreet();
            foreach (var i in Enumerable.Range(0, 10))
            {
                await clientStreamReq.RequestStream.WriteAsync(clientStremRequest);
                Console.WriteLine($"Sending req : {i} ...");
                await Task.Delay(100);
            }

            await clientStreamReq.RequestStream.CompleteAsync();
            var responseFormServerToCientStremRequest = await clientStreamReq.ResponseAsync;

            Console.WriteLine(responseFormServerToCientStremRequest.Result);

            Console.WriteLine("End CLIENT STREAMING GRPC test result");

            //CLIENT STREAMING EXERCICE
            Console.WriteLine("Start CLIENT STREAMING EXERICE");

            var calculatorClient = new CalculatorService.CalculatorServiceClient(channel);
            
            var numbers = new List<int> { 1, 2, 3, 4 };

            var avgRequest = calculatorClient.CalculateAverage();
            foreach(var number in numbers)
            {
                await avgRequest.RequestStream.WriteAsync(new AverageCalculatorRequest { Number = number });
            }
            await avgRequest.RequestStream.CompleteAsync();
            var avgResponse = await avgRequest.ResponseAsync;
            Console.WriteLine(avgResponse.Result);

            Console.WriteLine("End CLIENT STREAMING EXERICE");

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
