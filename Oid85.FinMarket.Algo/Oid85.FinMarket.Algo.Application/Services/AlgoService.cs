using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class AlgoService : IAlgoService
    {
        public async Task<StrategyBacktestResponse> StrategyBacktestAsync(StrategyBacktestRequest request)
        {
            return new ();
        }

        public async Task<StrategyOptimizationResponse> StrategyOptimizationAsync(StrategyOptimizationRequest request)
        {
            return new();
        }
    }
}
