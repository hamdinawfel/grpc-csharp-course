using Greeting;
using Grpc.Core;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
