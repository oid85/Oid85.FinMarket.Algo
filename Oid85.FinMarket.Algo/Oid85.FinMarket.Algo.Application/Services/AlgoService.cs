using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using Oid85.FinMarket.Algo.Application.Interfaces.Repositories;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Application.Mapping;
using Oid85.FinMarket.Algo.Common.KnownConstants;
using Oid85.FinMarket.Algo.Common.Utils;
using Oid85.FinMarket.Algo.Core.Configuration;
using Oid85.FinMarket.Algo.Core.Models;
using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class AlgoService(
        ILogger logger,
        IDataService dataService,
        IOptions<AlgoSettings> options,
        IStrategyExecuteResultRepository strategyExecuteResultRepository,
        IServiceProvider serviceProvider)
        : IAlgoService
    {
        /// <summary>
        /// Бэктест стратегий портфеля
        /// </summary>
        public async Task<BacktestResponse> BacktestAsync(BacktestRequest request)
        {
            request.PortfolioName = "PortfolioUltimateSmoother";

            string processName = KnownProcessNames.Backtest;

            await strategyExecuteResultRepository.DeleteAsync(request.PortfolioName, processName);

            var strategyExecuteResults = await ExecuteAsync(
                new()
                {
                    PortfolioName = request.PortfolioName,
                    IsOptimization = false,
                    ProcessName = processName
                });

            await strategyExecuteResultRepository.AddAsync(strategyExecuteResults);

            return new();
        }

        /// <summary>
        /// Оптимизация стратегий портфеля
        /// </summary>
        public async Task<OptimizationResponse> OptimizationAsync(OptimizationRequest request)
        {
            request.PortfolioName = "PortfolioUltimateSmoother";

            string processName = KnownProcessNames.Optimization;

            await strategyExecuteResultRepository.DeleteAsync(request.PortfolioName, processName);

            var strategyExecuteResults = await ExecuteAsync(
                new()
                {
                    PortfolioName = request.PortfolioName,
                    IsOptimization = true,
                    ProcessName = processName
                });

            await strategyExecuteResultRepository.AddAsync(strategyExecuteResults);

            return new();
        }

        /// <summary>
        /// Эмуляция проторговки портфеля стратегий
        /// </summary>
        public async Task<EmulateResponse> EmulateAsync(EmulateRequest request)
        {
            request.PortfolioName = "PortfolioUltimateSmoother";

            var strategyExecuteResults = await ExecuteAsync(
                new()
                {
                    PortfolioName = request.PortfolioName,
                    IsOptimization = false,
                    ProcessName = KnownProcessNames.Backtest
                });
            
            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * 30));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var dates = DateUtils.GetDates(from, to);

            var response = new EmulateResponse { Dates = dates };

            foreach (var strategyExecuteResult in strategyExecuteResults)
            {
                var positionList = new PositionList 
                {
                    Ticker = strategyExecuteResult.Ticker,
                    PositionListItems = dates
                    .Select(x => new PositionListItem
                    { 
                        Date = x,                         
                        PositionType = GetPositionType(strategyExecuteResult.DiagramPoints, x)
                    }).ToList()                    
                };

                response.PositionLists.Add(positionList);
            }

            return response;

            string? GetPositionType(List<DiagramPoint> diagramPoints, DateOnly date)
            {
                var diagramPoint = diagramPoints.Find(x => x.Date == date);

                if (diagramPoint is null) return null;

                if (diagramPoint.PositionDirection == 1) return "Long";
                if (diagramPoint.PositionDirection == -1) return "Short";

                return null;
            }
        }

        /// <summary>
        /// Выполнить стратегии портфеля
        /// </summary>
        private async Task<List<StrategyExecuteResult>> ExecuteAsync(StrategyExecuteRequest request)
        {
            var strategyExecuteResults = new List<StrategyExecuteResult>();

            var algoSettings = options.Value;
            var portfolioSettings = algoSettings.Portfolios.Find(x => x.Name == request.PortfolioName);
            var tickers = algoSettings.TickerLists.Find(x => x.Name == portfolioSettings!.TickerList)!.Tickers;
            var candleData = await GetCandleDataAsync(request.IsOptimization, tickers);
            var strategyData = GetStrategyData();

            foreach (var portfolioStrategySettings in portfolioSettings!.PortfolioStrategies)
            {
                var strategySettings = algoSettings.Strategies.Find(x => x.Name == portfolioStrategySettings.Name);
                var strategy = strategyData[portfolioStrategySettings.Name];

                foreach (var ticker in tickers)
                {
                    strategy.Ticker = ticker;
                    strategy.CandleData = candleData;
                    strategy.PortfolioName = portfolioSettings.Name;
                    strategy.StabilizationPeriod = algoSettings.BacktestSettings.StabilizationPeriodInCandles;
                    strategy.ProcessName = request.ProcessName!;

                    if (strategy.Candles is []) continue;

                    var parameterSets = request.IsOptimization
                        ? GetParameterSets(strategySettings!.StrategyParameters)
                        : await GetParameterSets(portfolioSettings.Name, strategySettings!.Name, ticker);

                    var results = Execute(strategy, parameterSets);

                    strategyExecuteResults.AddRange(results);
                }
            }

            return strategyExecuteResults;
        }

        /// <summary>
        /// Выполнить стратегию на наборах параметров
        /// </summary>
        private List<StrategyExecuteResult> Execute(Strategy strategy, List<Dictionary<string, int>> parameterSets)
        {
            var results = new List<StrategyExecuteResult>();

            foreach (var parameterSet in parameterSets)
            {
                var result = Execute(strategy, parameterSet);

                if (result is not null)
                    results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Выполнить стратегию на наборе параметров
        /// </summary>
        private StrategyExecuteResult? Execute(Strategy strategy, Dictionary<string, int> parameterSet)
        {
            var algoSettings = options.Value;
            var portfolioSettings = algoSettings.Portfolios.Find(x => x.Name == strategy.PortfolioName);

            StrategyExecuteResult result;

            try
            {
                if (parameterSet.Count == 0) return null;

                strategy.Init(parameterSet, portfolioSettings!.Money);
                strategy.Execute();
                result = ApplicationMapper.MapToStrategyExecuteResult(strategy);
                result.ResultMessage = "Success";
            }

            catch (Exception exception)
            {
                result = ApplicationMapper.MapToStrategyExecuteResult(strategy);
                result.ResultMessage = $"Error. {exception.Message}";
            }

            return result;
        }

        /// <summary>
        /// Получить стратегии
        /// </summary>
        private Dictionary<string, Strategy> GetStrategyData()
        {
            var algoSettings = options.Value;

            var strategyDictionary = new Dictionary<string, Strategy>();

            foreach (var strategySettings in algoSettings.Strategies)
            {
                var strategy = serviceProvider.GetRequiredKeyedService<Strategy>(strategySettings.Name);

                strategy.StrategyDescription = strategySettings.Description;
                strategy.StrategyName = strategySettings.Name;

                strategyDictionary.TryAdd(strategySettings.Name, strategy);
            }

            return strategyDictionary;
        }

        /// <summary>
        /// Получение свечей
        /// </summary>
        private async Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(bool isOptimization, List<string> tickers)
        {
            var dateRange = isOptimization ? GetOptimizationDates() : GetBacktestDates();

            var result = new Dictionary<string, List<Candle>>();

            var candleData = await dataService.GetCandleDataAsync(tickers);

            foreach (string ticker in tickers)
            {
                var candles = candleData[ticker]
                    .Where(x => x.Date >= dateRange.From)
                    .Where(x => x.Date <= dateRange.To)
                    .ToList();

                if (candles.Count == 0)
                    continue;

                for (int i = 0; i < candles.Count; i++)
                    candles[i].Index = i;

                result.TryAdd(ticker, candles);
            }

            return result;
        }

        /// <summary>
        /// Получить даты для оптимизации
        /// </summary>
        private (DateOnly From, DateOnly To) GetOptimizationDates()
        {
            var algoSettings = options.Value;

            var today = DateOnly.FromDateTime(DateTime.Today);

            var from = today
                .AddDays(-1 * algoSettings.BacktestSettings.BacktestWindowInDays)
                .AddDays(-1 * algoSettings.BacktestSettings.StabilizationPeriodInCandles)
                .AddDays(-1 * algoSettings.BacktestSettings.BacktestShiftInDays);

            var to = today.AddDays(-1 * algoSettings.BacktestSettings.BacktestShiftInDays);

            return (from, to);
        }

        /// <summary>
        /// Получить даты для бэктеста
        /// </summary>
        private (DateOnly From, DateOnly To) GetBacktestDates()
        {
            var algoSettings = options.Value;

            var today = DateOnly.FromDateTime(DateTime.Today);

            var from = today
                .AddDays(-1 * algoSettings.BacktestSettings.BacktestWindowInDays)
                .AddDays(-1 * algoSettings.BacktestSettings.StabilizationPeriodInCandles);

            var to = today;

            return (from, to);
        }

        /// <summary>
        /// Получить параметры стратегии для оптимизации
        /// </summary>
        private static List<Dictionary<string, int>> GetParameterSets(List<StrategyParameterSettings> strategyParams)
        {
            var result = new List<Dictionary<string, int>>();

            switch (strategyParams.Count)
            {
                case 1:
                    for (int paramValue1 = strategyParams[0].Min; paramValue1 <= strategyParams[0].Max; paramValue1 += strategyParams[0].Step)
                        result.Add(
                            new Dictionary<string, int>
                            {
                                [strategyParams[0].Name] = paramValue1
                            });

                    return result;

                case 2:
                    for (int paramValue1 = strategyParams[0].Min; paramValue1 <= strategyParams[0].Max; paramValue1 += strategyParams[0].Step)
                        for (int paramValue2 = strategyParams[1].Min; paramValue2 <= strategyParams[1].Max; paramValue2 += strategyParams[1].Step)
                            result.Add(
                                new Dictionary<string, int>
                                {
                                    [strategyParams[0].Name] = paramValue1,
                                    [strategyParams[1].Name] = paramValue2
                                });

                    return result;

                case 3:
                    for (int paramValue1 = strategyParams[0].Min; paramValue1 <= strategyParams[0].Max; paramValue1 += strategyParams[0].Step)
                        for (int paramValue2 = strategyParams[1].Min; paramValue2 <= strategyParams[1].Max; paramValue2 += strategyParams[1].Step)
                            for (int paramValue3 = strategyParams[2].Min; paramValue3 <= strategyParams[2].Max; paramValue3 += strategyParams[2].Step)
                                result.Add(
                                    new Dictionary<string, int>
                                    {
                                        [strategyParams[0].Name] = paramValue1,
                                        [strategyParams[1].Name] = paramValue2,
                                        [strategyParams[2].Name] = paramValue3
                                    });

                    return result;
            }

            throw new Exception("Количество параметров не может быть больше трёх");
        }

        /// <summary>
        /// Получить параметры стратегии для бэктеста
        /// </summary>
        private async Task<List<Dictionary<string, int>>> GetParameterSets(string portfolioName, string strategyName, string ticker)
        {
            var strategyExecuteResults = (await strategyExecuteResultRepository.GetFilteredAsync())
                .Where(x => x.PortfolioName == portfolioName)
                .Where(x => x.StrategyName == strategyName)
                .Where(x => x.Ticker == ticker)
                .ToList();

            if (strategyExecuteResults is [])
                return [];

            var parameterSets = strategyExecuteResults
                .Select(x => JsonSerializer.Deserialize<Dictionary<string, int>>(x.StrategyParams))
                .ToList();

            return parameterSets;
        }
    }
}
