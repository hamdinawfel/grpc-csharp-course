using Dummy;
using Greeting;
using Grpc.Core;
using Sum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ///SECTION 1
            //var client = new DummyService.DummyServiceClient(channel);

            ///SECTION 2
            //var client = new GreetingService.GreetingServiceClient(channel);

            //var greeting = new Greeting.Greeting()
            //{
            //    FirstName = "Nawfel",
            //    LastName = "Hamdi"
            //};

            //var request = new GreetingRequest() { Greeting = greeting };
            //var response = client.Greet(request);

            //Console.WriteLine(response.Result);

            ///SECTION 2 - EXCERCICE
            var client = new SumService.SumServiceClient(channel);

            

            var request = new SumRequest() {  A = 10, B = 3 };
            var response = client.Sum(request);

            Console.WriteLine(response.Result);


            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
