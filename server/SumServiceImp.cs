using Grpc.Core;
using Sum;
using System.Threading.Tasks;
using static Sum.SumService;

namespace server
{
    public class SumServiceImp : SumServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            var result = request.A + request.B;

            return Task.FromResult(new SumResponse() { Result = result });
        }
    }
}
