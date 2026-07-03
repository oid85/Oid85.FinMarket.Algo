using Microsoft.Extensions.Options;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Configuration;
using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class AlgoService(
        IOptions<AlgoSettings> options)
        : IAlgoService
    {
        public async Task<StrategyBacktestResponse> StrategyBacktestAsync(StrategyBacktestRequest request)
        {            
            return new ();
        }

        public async Task<StrategyOptimizationResponse> StrategyOptimizationAsync(StrategyOptimizationRequest request)
        {
            var algoSettings = options.Value;
            
            return new();
        }
    }
}
