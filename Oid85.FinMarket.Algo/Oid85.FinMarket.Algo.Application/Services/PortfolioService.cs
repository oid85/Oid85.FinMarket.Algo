using System.Linq;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Common.KnownConstants;
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

            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * 365));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var dates = DateUtils.GetDates(from, to);

            double money = startMoney;
            double totalSum = startMoney;

            List<string> tickers = [.. strategyExecuteResults.Select(x => x.Ticker).Distinct(), KnownTickers.TMON];

            var candleData = await dataService.GetCandleDataAsync(tickers);
            var instrumentData = await dataService.GetInstrumentDataAsync(tickers);

            List<(string Ticker, List<(DateOnly Date, int Weight)> WeightData)> positionDirectionData = strategyExecuteResults
                .Select(x => (x.Ticker, x.DiagramPoints.Select(xx => (xx.Date, xx.PositionDirection ?? 0)).ToList()))
                .ToList();

            var weights = tickers.ToDictionary(k => k, v => 0);
            var sizes = tickers.ToDictionary(k => k, v => 0);
            var costs = tickers.ToDictionary(k => k, v => 0.0);
            var prices = tickers.ToDictionary(k => k, v => 0.0);
            var lots = instrumentData.ToDictionary(k => k.Key, v => v.Value.Lot ?? 1);

            var portfolioDiagram = new PortfolioDiagram();

            foreach (var date in dates)
            {
                UpdateWeights();
                UpdatePrices();
                UpdateCosts();
                UpdateTotalSum();
                Rebalance();

                portfolioDiagram.EqiutyCurve.Add(new DateValue<double> { Date = date, Value = totalSum });
                
                if (portfolioDiagram.EqiutyCurve.Count > 0)
                {
                    var maxEquity = portfolioDiagram.EqiutyCurve.MaxBy(x => x.Value);
                    var currentEquity = portfolioDiagram.EqiutyCurve[^1];
                    var currentDrawdown = -1 * (maxEquity!.Value - currentEquity.Value);

                    portfolioDiagram.DrawdownCurve.Add(new DateValue<double> { Date = currentEquity.Date, Value = currentDrawdown });
                }
                
                void UpdateWeights()
                {
                    List<(string Ticker, int Weight)> positionData = [.. positionDirectionData.Select(x => (x.Ticker, x.WeightData.FindLast(xx => xx.Date <= date).Weight))];
                    
                    weights = tickers.ToDictionary(k => k, v => positionData.Where(x => x.Ticker == v).Sum(x => x.Weight));
                    int count = weights.Values.Sum();
                    weights[KnownTickers.TMON] = strategyExecuteResults.Count - count;
                }

                void UpdatePrices()
                {
                    foreach (var ticker in tickers)
                        prices[ticker] = dataService.GetPrice(ticker, date) ?? 0.0;
                }

                void UpdateCosts()
                {
                    foreach (var ticker in tickers)
                        costs[ticker] = prices[ticker] * sizes[ticker];
                }

                void UpdateSizes()
                {
                    double baseUnit = totalSum / weights.Values.Sum();

                    foreach (var ticker in tickers)
                    {
                        if (prices[ticker] == 0.0)
                        {
                            costs[ticker] = 0.0;
                            continue;
                        }

                        double tickerCost = baseUnit * weights[ticker];
                        double tickerSize = tickerCost / prices[ticker];
                        tickerSize /= lots[ticker];
                        tickerSize = Math.Truncate(tickerSize);
                        tickerSize *= lots[ticker];
                        sizes[ticker] = Convert.ToInt32(tickerSize);
                    }
                }

                void UpdateTotalSum()
                {
                    totalSum = costs.Values.Sum() + money;
                }

                void Rebalance()
                {
                    UpdateSizes();
                    UpdateCosts();
                    money = totalSum - costs.Values.Sum();
                }
            }

            return portfolioDiagram;
        }
    }
}
