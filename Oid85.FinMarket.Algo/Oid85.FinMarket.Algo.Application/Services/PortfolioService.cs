using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Common.Utils;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class PortfolioService(
        IDataService dataService) 
        : IPortfolioService
    {
        public Dictionary<string, PortfolioPosition> GetCurrentPortfolioPositions(List<StrategyExecuteResult> strategyExecuteResults)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<PortfolioPosition>> GetPortfolioPositions(List<StrategyExecuteResult> strategyExecuteResults)
        {
            throw new NotImplementedException();
        }

        public async Task<PortfolioDiagram> GetPortfolioDiagramAsync(List<StrategyExecuteResult> strategyExecuteResults)
        {
            if (strategyExecuteResults is [])
                return new();

            double startMoney = 1_000_000.0;

            var startDate = strategyExecuteResults[0].StartDate;
            var endDate = strategyExecuteResults[0].EndDate;

            var dates = DateUtils.GetDates(startDate, endDate);

            double money = startMoney;
            double totalSum = startMoney;

            var tickers = strategyExecuteResults.Select(x => x.Ticker).Distinct().ToList();
            var candleData = await dataService.GetCandleDataAsync([.. tickers, "TMON"]);
            var instrumentData = await dataService.GetInstrumentDataAsync([..tickers, "TMON"]);

            var weight = tickers.ToDictionary(k => k, v => 0);
            var sizes = tickers.ToDictionary(k => k, v => 0);
            var costs = tickers.ToDictionary(k => k, v => 0.0);
            var prices = tickers.ToDictionary(k => k, v => 0.0);
            var lots = instrumentData.ToDictionary(k => k.Key, v => v.Value.Lot ?? 1);

            return new();
        }
    }
}
