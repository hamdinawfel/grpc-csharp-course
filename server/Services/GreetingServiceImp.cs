using Greeting;
using Grpc.Core;
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
    }
}
