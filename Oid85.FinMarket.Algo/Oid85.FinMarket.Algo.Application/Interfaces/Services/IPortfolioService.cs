using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioData> GetPortfolioDataAsync(string portfolioName, List<StrategyExecuteResult> strategyExecuteResults);
    }
}
