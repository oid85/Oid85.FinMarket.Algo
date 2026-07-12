using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioDiagram> GetPortfolioDiagramAsync(List<StrategyExecuteResult> strategyExecuteResults);
        Dictionary<string, List<PortfolioPosition>> GetPortfolioPositions(List<StrategyExecuteResult> strategyExecuteResults);
        Dictionary<string, PortfolioPosition> GetCurrentPortfolioPositions(List<StrategyExecuteResult> strategyExecuteResults);
    }
}
