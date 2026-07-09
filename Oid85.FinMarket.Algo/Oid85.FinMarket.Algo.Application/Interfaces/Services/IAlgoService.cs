using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IAlgoService
    {
        /// <summary>
        /// Бэктест стратегий портфеля
        /// </summary>
        Task<BacktestResponse> BacktestAsync(BacktestRequest request);

        /// <summary>
        /// Оптимизация стратегий портфеля
        /// </summary>
        Task<OptimizationResponse> OptimizationAsync(OptimizationRequest request);

        /// <summary>
        /// Мониторинг стратегий
        /// </summary>
        Task<MonitorResponse> MonitorAsync(MonitorRequest request);
        
        /// <summary>
        /// Список портфелей
        /// </summary>
        Task<PortfolioListResponse> PortfolioListAsync(PortfolioListRequest request);
    }
}
