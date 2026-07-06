using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Repositories
{
    public interface IStrategyExecuteResultRepository
    {
        Task AddAsync(List<StrategyExecuteResult> strategyExecuteResults);
        Task<List<StrategyExecuteResult>> GetAsync();
        Task DeleteAsync(string portfolioName);
    }
}
