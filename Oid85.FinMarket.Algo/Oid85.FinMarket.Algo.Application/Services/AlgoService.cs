using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NLog;
using Oid85.FinMarket.Algo.Application.Helpers;
using Oid85.FinMarket.Algo.Application.Interfaces.Repositories;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Application.Mapping;
using Oid85.FinMarket.Algo.Core.Configuration;
using Oid85.FinMarket.Algo.Core.Models;
using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class AlgoService(
        ILogger logger,
        IOptions<AlgoSettings> options,
        IStrategyExecuteResultRepository strategyExecuteResultRepository,
        AlgoHelper algoHelper)
        : IAlgoService
    {
        public async Task<PortfolioBacktestResponse> PortfolioBacktestAsync(PortfolioBacktestRequest request)
        {
            request.PortfolioName = "PortfolioUltimateSmoother";

            return new();
        }

        public async Task<PortfolioOptimizationResponse> PortfolioOptimizationAsync(PortfolioOptimizationRequest request)
        {
            request.PortfolioName = "PortfolioUltimateSmoother";

            var algoSettings = options.Value;
            var portfolioSettings = algoSettings.Portfolios.Find(x => x.Name == request.PortfolioName);
            var tickers = algoSettings.TickerLists.Find(x => x.Name == portfolioSettings!.TickerList)!.Tickers;
            var candleData = await algoHelper.GetCandleDataAsync(isOptimization: true, tickers);
            var strategyInstanceData = algoHelper.GetStrategyData();

            await strategyExecuteResultRepository.DeleteAsync(portfolioSettings!.Name);

            foreach (var portfolioStrategySettings in portfolioSettings!.PortfolioStrategies)
            {
                var strategySettings = algoSettings.Strategies.Find(x => x.Name == portfolioStrategySettings.Name);
                var strategyInstance = strategyInstanceData[portfolioStrategySettings.Name];

                var strategyExecuteResults = new List<StrategyExecuteResult>();

                foreach (var ticker in tickers)
                {
                    strategyInstance.Ticker = ticker;
                    strategyInstance.Leverage = portfolioSettings.Leverage;
                    strategyInstance.CandleData = candleData;
                    strategyInstance.PortfolioName = portfolioSettings.Name;
                    strategyInstance.StabilizationPeriod = algoSettings.BacktestSettings.StabilizationPeriodInCandles;
                    strategyInstance.ProcessName = "Optimization";

                    if (strategyInstance.Candles is []) continue;

                    var parameterSets = AlgoHelper.GetParameterSets(strategySettings!.StrategyParameters);

                    foreach (var parameterSet in parameterSets)
                    {
                        StrategyExecuteResult strategyExecuteResult;

                        try
                        {
                            if (parameterSet.Count == 0) continue;

                            strategyInstance.InitForParameterSet(parameterSet, portfolioSettings.Money);                            
                            strategyInstance.Execute();
                            strategyExecuteResult = ApplicationMapper.MapToStrategyExecuteResult(strategyInstance);
                            strategyExecuteResult.ResultMessage = "Success";                            
                        }

                        catch (Exception exception)
                        {
                            logger.Info($"Оптимизация '{strategyInstance.Ticker}', '{strategyInstance.StrategyName}', '{JsonSerializer.Serialize(parameterSet)}'. {exception}");

                            strategyExecuteResult = ApplicationMapper.MapToStrategyExecuteResult(strategyInstance);
                            strategyExecuteResult.ResultMessage = $"Error. {exception.Message}";
                        }

                        strategyExecuteResults.Add(strategyExecuteResult);
                    }
                }

                if (strategyExecuteResults is not [])
                    await strategyExecuteResultRepository.AddAsync(strategyExecuteResults);
            }

            return new();
        }
    }
}
