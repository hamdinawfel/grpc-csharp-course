using Greeting;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Greeting.GreetingService;

namespace server
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
