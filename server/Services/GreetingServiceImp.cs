using Greeting;
using Grpc.Core;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using static Greeting.GreetingService;

namespace server.Services
{
    public class GreetingServiceImp : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            var result = $"Hello {request.Greeting.FirstName} {request.Greeting.LastName}";


            return Task.FromResult(new GreetingResponse() {  Result = result });
        }

        public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            var result = $"Hello {request.Greeting.FirstName} {request.Greeting.LastName}";

            foreach (var i in Enumerable.Range(0, 10))
            {
                await responseStream.WriteAsync(new GreetingManyTimesResponse() { Result = result });   
            }
        }

        public override async Task<LongGreetingResponse> LongGreet(IAsyncStreamReader<LongGreetingRequest> requestStream, ServerCallContext context)
        {
            var result = "";
            while (await requestStream.MoveNext())
            {
                result += $"{requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName} {Environment.NewLine}";
            }

            return new LongGreetingResponse() { Result = result };  
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetingEveryoneRequest> requestStream, IServerStreamWriter<GreetingEveryoneResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var result = $"{requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}";
                await Console.Out.WriteLineAsync($"Received : {result}");
                await responseStream.WriteAsync(new GreetingEveryoneResponse() { Result = result });
            }
        }
    }
}
