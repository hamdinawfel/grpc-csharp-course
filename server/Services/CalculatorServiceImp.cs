using Calculator;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Calculator.CalculatorService;

namespace server.Services
{
    public class CalculatorServiceImp : CalculatorServiceBase
    {
        public override async Task<AverageCalculatorResponse> CalculateAverage(IAsyncStreamReader<AverageCalculatorRequest> requestStream, ServerCallContext context)
        {
            var numbers = new List<int> ();

            while (await requestStream.MoveNext())
            {
                numbers.Add(requestStream.Current.Number);
            }

            double average = numbers.Average();

            return new AverageCalculatorResponse { Result = average };
        }
    }
}
