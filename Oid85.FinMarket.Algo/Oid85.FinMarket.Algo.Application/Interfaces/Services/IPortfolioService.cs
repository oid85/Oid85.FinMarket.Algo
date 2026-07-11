using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IPortfolioService
    {
        PortfolioDiagram GetPortfolioDiagram(List<StrategyExecuteResult> strategyExecuteResults);
        Dictionary<string, PortfolioPosition> GetPortfolioCurrentPositions(List<StrategyExecuteResult> strategyExecuteResults);
    }
}
