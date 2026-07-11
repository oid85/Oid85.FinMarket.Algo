using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        public Dictionary<string, PortfolioPosition> GetCurrentPortfolioPositions(List<StrategyExecuteResult> strategyExecuteResults)
        {
            throw new NotImplementedException();
        }

        public PortfolioDiagram GetPortfolioDiagram(List<StrategyExecuteResult> strategyExecuteResults)
        {
            throw new NotImplementedException();
        }
    }
}
