using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Configuration;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Helpers;

public class AlgoHelper(
    IDataService dataService,
    IOptions<AlgoSettings> options,
    IServiceProvider serviceProvider)
{
    /// <summary>
    /// Получить стратегии
    /// </summary>
    public Dictionary<string, Strategy> GetStrategies()
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
    public async Task<Dictionary<string, List<Candle>>> GetCandlesAsync(bool isOptimization, List<string> tickers)
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
    /// Получить параметры стратегии
    /// </summary>
    /// <param name="strategyParams">Параметры из ресурсов</param>
    public static List<Dictionary<string, int>> GetParameterSets(List<StrategyParameterSettings> strategyParams)
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

        throw new Exception("Количество параметров больше 3. Оптимизация выполняться не будет");
    }         
}