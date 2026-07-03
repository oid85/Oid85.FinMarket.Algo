using Accord.Statistics.Models.Regression.Linear;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Oid85.FinMarket.Application.Interfaces.Repositories;
using Oid85.FinMarket.Common.Utils;
using Oid85.FinMarket.Domain.Mapping;
using Oid85.FinMarket.Domain.Models.Algo;
using Oid85.FinMarket.Domain.Models.Analyse;
using Oid85.FinMarket.Domain.Models.PairArbitrage;
using Oid85.FinMarket.External.Computation;
using Oid85.FinMarket.External.ResourceStore;
using Oid85.FinMarket.External.ResourceStore.Models.Algo;

namespace Oid85.FinMarket.Application.Helpers;

public class AlgoHelper(
    ILogger logger,
    IServiceProvider serviceProvider,
    IResourceStoreService resourceStoreService,
    IRegressionTailRepository regressionTailRepository,
    IDailyCandleRepository dailyCandleRepository,
    ICorrelationRepository correlationRepository,
    IComputationService computationService,
    IFutureRepository futureRepository)
{
    /// <summary>
    /// Получить даты для оптимизации
    /// </summary>
    private async Task<(DateOnly From, DateOnly To)> GetOptimizationDatesAsync()
    {
        var algoConfigResource = await resourceStoreService.GetAlgoConfigAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var from = today
            .AddDays(-1 * algoConfigResource.PeriodConfigResource.BacktestWindowInDays)
            .AddDays(-1 * algoConfigResource.PeriodConfigResource.DailyStabilizationPeriodInDays)
            .AddDays(-1 * algoConfigResource.PeriodConfigResource.BacktestShiftInDays);
            
        var to = today.AddDays(-1 * algoConfigResource.PeriodConfigResource.BacktestShiftInDays);

        return (from, to);
    }
    
    /// <summary>
    /// Получить даты для бэктеста
    /// </summary>
    private async Task<(DateOnly From, DateOnly To)> GetBacktestDatesAsync()
    {
        var algoConfigResource = await resourceStoreService.GetAlgoConfigAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var from = today
            .AddDays(-1 * algoConfigResource.PeriodConfigResource.BacktestWindowInDays)
            .AddDays(-1 * algoConfigResource.PeriodConfigResource.DailyStabilizationPeriodInDays);
            
        var to = today;

        return (from, to);
    }   
    
    /// <summary>
    /// Получить тикеры для Алго
    /// </summary>
    private async Task<List<string>> GetAllTickersForAlgoAsync()
    {
        var algoStrategyResources = await resourceStoreService.GetAlgoStrategiesAsync();
        
        var tickers = new List<string>();

        foreach (var algoStrategyResource in algoStrategyResources)
        {
            var tickersInTickerList = (await resourceStoreService.GetTickerListAsync(algoStrategyResource.TickerList)).Tickers;

            foreach (var ticker in tickersInTickerList.Where(ticker => !tickers.Contains(ticker))) 
                tickers.Add(ticker);
        }
        
        return tickers;
    }    
    
    /// <summary>
    /// Получить тикеры для Парного арбитража
    /// </summary>
    private async Task<List<string>> GetAllTickersForPairArbitrageAsync()
    {
        var tails = await regressionTailRepository.GetAllAsync();
        
        List<string> tickers = 
                [
                    ..tails.Select(x => x.TickerFirst).ToList(),
                    ..tails.Select(x => x.TickerSecond).ToList()
                ];
        
        return tickers.Distinct().ToList();
    }
    
    /// <summary>
    /// Получить стратегии для Алго
    /// </summary>
    /// <param name="strategyId">Id стратегии</param>
    public async Task<Dictionary<Guid, Strategy>> GetAlgoStrategies(Guid? strategyId = null)
    {
        var algoStrategyResources = strategyId is null 
            ? await resourceStoreService.GetAlgoStrategiesAsync() 
            : (await resourceStoreService.GetAlgoStrategiesAsync()).Where(x => x.Id == strategyId);
        
        var strategyDictionary = new Dictionary<Guid, Strategy>();
        
        foreach (var algoStrategyResource in algoStrategyResources)
        {
            var strategy = serviceProvider.GetRequiredKeyedService<Strategy>(algoStrategyResource.Name);
                
            strategy.StrategyId = algoStrategyResource.Id;
            strategy.Timeframe = algoStrategyResource.Timeframe;
            strategy.StrategyDescription = algoStrategyResource.Description;
            strategy.StrategyName = algoStrategyResource.Name;
                
            strategyDictionary.TryAdd(algoStrategyResource.Id, strategy);
        }

        return strategyDictionary;
    }
    
    /// <summary>
    /// Получить стратегии для Парного арбитража
    /// </summary>
    /// <param name="strategyId">Id стратегии</param>
    public async Task<Dictionary<Guid, PairArbitrageStrategy>> GetPairArbitrageStrategies(Guid? strategyId = null)
    {
        var pairArbitrageStrategyResources = strategyId is null
            ? await resourceStoreService.GetPairArbitrageStrategiesAsync() 
            : (await resourceStoreService.GetPairArbitrageStrategiesAsync()).Where(x => x.Id == strategyId);

        var strategyDictionary = new Dictionary<Guid, PairArbitrageStrategy>();        
        
        foreach (var pairArbitrageStrategyResource in pairArbitrageStrategyResources)
        {
            var strategy = serviceProvider.GetRequiredKeyedService<PairArbitrageStrategy>(pairArbitrageStrategyResource.Name);

            strategy.StrategyId = pairArbitrageStrategyResource.Id;
            strategy.Timeframe = pairArbitrageStrategyResource.Timeframe;
            strategy.StrategyDescription = pairArbitrageStrategyResource.Description;
            strategy.StrategyName = pairArbitrageStrategyResource.Name;

            strategyDictionary.TryAdd(pairArbitrageStrategyResource.Id, strategy);
        }
        
        return strategyDictionary;
    }  
    
    /// <summary>
    /// Получение свечей для Алго
    /// </summary>
    /// <param name="isOptimization">Признак выполнения процесса оптимизации</param>
    /// <param name="ticker">Тикер инструмента</param>
    public async Task<Dictionary<string, List<Candle>>> GetAlgoCandlesAsync(bool isOptimization, string? ticker = null)
    {
        var dates = isOptimization ? await GetOptimizationDatesAsync() : await GetBacktestDatesAsync();

        var tickers = ticker is null ? await GetAllTickersForAlgoAsync() : [ticker];
        
        var result = new Dictionary<string, List<Candle>>();
        
        foreach (string instrumentTicker in tickers)
        {
            var candles = (await dailyCandleRepository.GetAsync(instrumentTicker, dates.From, dates.To))
                .Select(AlgoMapper.Map).ToList();
            
            if (candles.Count == 0)
                continue;

            for (int i = 0; i < candles.Count; i++)
                candles[i].Index = i;
            
            result.TryAdd(instrumentTicker, candles);
        }

        return result;
    }
    
    /// <summary>
    /// Получение свечей для Парного арбитража
    /// </summary>
    /// <param name="isOptimization">Признак выполнения процесса оптимизации</param>
    /// <param name="ticker1">Тикер первого инструмента</param>
    /// <param name="ticker2">Тикер второго инструмента</param>
    public async Task<Dictionary<string, List<Candle>>> GetPairArbitrageCandlesAsync(bool isOptimization, string? ticker1 = null, string? ticker2 = null)
    {
        var dates = isOptimization ? await GetOptimizationDatesAsync() : await GetBacktestDatesAsync();

        var tickers = ticker1 is null || ticker2 is null 
            ? await GetAllTickersForPairArbitrageAsync() 
            : [ticker1, ticker2];

        var result = new Dictionary<string, List<Candle>>();
        
        foreach (string instrumentTicker in tickers)
        {
            var candles = (await dailyCandleRepository.GetAsync(instrumentTicker, dates.From, dates.To))
                .Select(AlgoMapper.Map).ToList();

            if (candles.Count == 0)
                continue;

            for (int i = 0; i < candles.Count; i++)
                candles[i].Index = i;
            
            result.TryAdd(instrumentTicker, candles);
        }
        
        return result;
    }    
    
    /// <summary>
    /// Получить спреды
    /// </summary>
    /// <param name="ticker1">Тикер первого инструмента</param>
    /// <param name="ticker2">Тикер второго инструмента</param>
    public async Task<Dictionary<string, RegressionTail>> GetSpreadsAsync(string? ticker1 = null, string? ticker2 = null)
    {
        var tails = ticker1 is null || ticker2 is null 
            ? await regressionTailRepository.GetAllAsync()
            : (await regressionTailRepository.GetAllAsync()).Where(x => x.TickerFirst == ticker1 && x.TickerSecond == ticker2);

        var spreads = new Dictionary<string, RegressionTail>();
        
        foreach (var tail in tails)
        {
            if (tail.Tails.Count == 0)
                continue;

            spreads.TryAdd($"{tail.TickerFirst},{tail.TickerSecond}", tail);
        }

        return spreads;
    } 
    
    /// <summary>
    /// Получить параметры стратегии
    /// </summary>
    /// <param name="strategyParams">Параметры из ресурсов</param>
    public static List<Dictionary<string, int>> GetParameterSets(List<StrategyParamResource> strategyParams)
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
    
    /// <summary>
    /// Z-score
    /// </summary>
    private static List<RegressionTailItem> ZScore(List<RegressionTailItem> values)
    {
        if (values.Count == 0)
            return [];

        var dates = values.Select(x => x.Date).ToList();
        var tailValues = values.Select(x => x.Value).ToList();

        var average = tailValues.Average();
        var stdDev = tailValues.StdDev();

        if (stdDev == 0.0)
            return [];

        var zScoreValues = tailValues.AddConst(-1 * average).DivConst(stdDev);

        var result = new List<RegressionTailItem>();

        for (int i = 0; i < dates.Count; i++)
            result.Add(new RegressionTailItem { Date = dates[i], Value = zScoreValues[i] });

        return result;
    }
    
    /// <summary>
    /// Синхронизация свечей
    /// </summary>
    public static (List<DailyCandle> First, List<DailyCandle> Second) SyncCandles(List<DailyCandle> candles1, List<DailyCandle> candles2)
    {
        var dates1 = candles1.Select(x => x.Date).ToList();
        var dates2 = candles2.Select(x => x.Date).ToList();

        var dates = dates1.Intersect(dates2).ToList();

        var resultCandles1 = candles1.Where(x => dates.Contains(x.Date)).OrderBy(x => x.Date).ToList();
        var resultCandles2 = candles2.Where(x => dates.Contains(x.Date)).OrderBy(x => x.Date).ToList();

        return (resultCandles1, resultCandles2);
    }

    /// <summary>
    /// Синхронизация свечей
    /// </summary>
    public static (List<Candle> First, List<Candle> Second) SyncCandles(List<Candle> candles1, List<Candle> candles2)
    {
        var dates1 = candles1.Select(x => x.DateTime.Date).ToList();
        var dates2 = candles2.Select(x => x.DateTime.Date).ToList();

        var dates = dates1.Intersect(dates2).ToList();

        var resultCandles1 = candles1.Where(x => dates.Contains(x.DateTime.Date)).OrderBy(x => x.DateTime.Date).ToList();
        var resultCandles2 = candles2.Where(x => dates.Contains(x.DateTime.Date)).OrderBy(x => x.DateTime.Date).ToList();

        return (resultCandles1, resultCandles2);
    }     
    
    /// <summary>
    /// Рассчитать хвосты регрессии за период
    /// </summary>
    public async Task<Dictionary<string, RegressionTail>> CalculateRegressionTailsAsync(DateOnly from, DateOnly to)
    {
        // Очистим таблицу
        await regressionTailRepository.DeleteAsync();

        var correlations = (await correlationRepository.GetAllAsync()).ToList();

        var tails = new Dictionary<string, RegressionTail>();

        foreach (var correlation in correlations)
        {
            try
            {
                // Получаем и синхронизируем свечи
                var syncCandles = SyncCandles(
                    await dailyCandleRepository.GetAsync(correlation.TickerFirst, from, to), 
                    await dailyCandleRepository.GetAsync(correlation.TickerSecond, from, to));

                // Declare some sample test data.
                double[] inputs = syncCandles.Second.Select(x => x.Close).ToArray();
                double[] outputs = syncCandles.First.Select(x => x.Close).ToArray();

                // Use Ordinary Least Squares to learn the regression
                var ols = new OrdinaryLeastSquares();

                // Use OLS to learn the simple linear regression
                SimpleLinearRegression regression = ols.Learn(inputs, outputs);

                // We can also extract the slope and the intercept term for the line
                double slope = regression.Slope;
                double intercept = regression.Intercept;

                string key = $"{correlation.TickerFirst},{correlation.TickerSecond}";

                // Расчет хвостов
                var regressionTails = new List<RegressionTailItem>();

                for (int i = 0; i < syncCandles.First.Count; i++)
                {
                    double y = slope * syncCandles.Second[i].Close + intercept;
                    double tailValue = syncCandles.First[i].Close - y;

                    if (!tails.ContainsKey(key))
                        tails.Add(key, new RegressionTail { TickerFirst = correlation.TickerFirst, TickerSecond = correlation.TickerSecond });

                    regressionTails.Add(new RegressionTailItem
                        { Date = syncCandles.First[i].Date, Value = tailValue });
                }

                tails[key].Slope = slope;
                tails[key].Intercept = intercept;

                // Расчитаем Z-score
                tails[key].Tails = ZScore(regressionTails);

                // Проверяем на стационарность и сохраняем
                var isStationary = await computationService.CheckStationaryAsync(
                    [tails[key].Tails.Select(x => x.Value).ToList()]);

                if (isStationary[0])
                    await regressionTailRepository.AddAsync(tails[key]);
                else
                    tails.Remove(key);
            }

            catch (Exception exception)
            {
                logger.Error(exception, "Ошибка расчета остатков регрессии. {tickerFirst}, {tickerSecond}", correlation.TickerFirst, correlation.TickerSecond);
            }
        }

        return tails;
    }
    
    /// <summary>
    /// Признак фьючерса
    /// </summary>
    /// <param name="ticker">Тикер инструмента</param>
    public async Task<bool> IsFuture(string ticker) => await futureRepository.GetAsync(ticker) is not null;

    /// <summary>
    /// Получить размер основного актива
    /// </summary>
    /// <param name="ticker">Тикер инструмента</param>
    public async Task<double> GetBasicAssetSize(string ticker) => (await futureRepository.GetAsync(ticker))?.BasicAssetSize ?? 1.0;

    /// <summary>
    /// Размер плеча
    /// </summary>
    /// <param name="ticker">Тикер инструмента</param>
    public async Task<double> GetLeverage(string ticker)
    {
        var algoConfigResource = await resourceStoreService.GetAlgoConfigAsync();

        return await IsFuture(ticker)
            ? algoConfigResource.MoneyManagementResource.FutureLeverage
            : algoConfigResource.MoneyManagementResource.ShareLeverage;
    }

    /// <summary>
    /// Размер плеча для парного арбитража
    /// </summary>
    public async Task<(double First, double Second)> GetPairArbitrageLeverage(string tickerFirst, string tickerSecond)
    {
        var algoConfigResource = await resourceStoreService.GetAlgoConfigAsync();
        
        var leverageFirst =  await IsFuture(tickerFirst)
            ? algoConfigResource.MoneyManagementResource.PairArbitrageFutureLeverage
            : algoConfigResource.MoneyManagementResource.PairArbitrageShareLeverage;
        
        var leverageSecond =  await IsFuture(tickerSecond)
            ? algoConfigResource.MoneyManagementResource.PairArbitrageFutureLeverage
            : algoConfigResource.MoneyManagementResource.PairArbitrageShareLeverage;        
        
        return (leverageFirst, leverageSecond);
    }
}