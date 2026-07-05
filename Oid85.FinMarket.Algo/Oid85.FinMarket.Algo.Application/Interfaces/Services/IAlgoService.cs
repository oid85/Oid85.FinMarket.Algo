using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IAlgoService
    {
        Task<PortfolioBacktestResponse> PortfolioBacktestAsync(PortfolioBacktestRequest request);
        Task<PortfolioOptimizationResponse> PortfolioOptimizationAsync(PortfolioOptimizationRequest request);
    }
}
