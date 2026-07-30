using Microsoft.Extensions.Caching.Memory;
using Oid85.FinMarket.Algo.Common.Utils;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    public class MomentumControlRiskLong(
        IMemoryCache memoryCache) 
        : Strategy
    {
        public override void Execute()
        {
            // Получаем параметры
            int period = Parameters["Period"];
            int percent = Parameters["Percent"];

            for (int i = StabilizationPeriod; i < Candles.Count - 1; i++)
            {
                // Торгуем только в начале месяца
                bool isBalancing = Candles[i].Date.Month == Candles[i - 1].Date.Month + 1;
                
                if (isBalancing)
                {
                    var topTickers = GetTopTickers(Candles[i].Date, period, percent);

                    bool tickerInTop = topTickers.Contains(Ticker);

                    // Правило входа
                    SignalLong = tickerInTop;

                    // Правило выхода
                    SignalCloseLong = !tickerInTop;

                    // Задаем цену для заявки
                    double orderPrice = Candles[i].Close;

                    // Расчет размера позиции
                    int positionSize = GetPositionSize(orderPrice);

                    if (LastActivePosition is null)
                    {
                        if (SignalLong && FilterLong)
                            BuyAtPrice(positionSize, orderPrice, i + 1);
                    }

                    else
                    {
                        if (SignalCloseLong)
                            SellAtPrice(positionSize, orderPrice, i + 1);
                    }
                }

                else
                {
                    if (LastActivePosition is not null)
                    {
                        double profit = Math.Abs(LastActivePosition.Quantity) * (Candles[i].Close - LastActivePosition.EntryPrice);

                        // Если имеем убыток
                        if (profit < 0)
                        {
                            double lossPercent = Math.Abs(profit / EndMoney * 100.0);

                            if (lossPercent >= 2.0)
                                SignalCloseLong = true;

                        }

                        if (SignalCloseLong)
                            SellAtPrice(Math.Abs(LastActivePosition.Quantity), Candles[i].Close, i + 1);
                    }
                }
                
                // Отрисовка
                DiagramPoints[i].Price = Candles[i].Close;
            }
        }

        private List<string> GetTopTickers(DateOnly date, int period, int percent)
        {
            string key = StringUtils.GetMd5($"GetTopTickers_{date}_{period}_{percent}");

            if (memoryCache.TryGetValue(key, out List<string>? cacheTopTickers))
                return cacheTopTickers ?? [];

            DateOnly from = date.AddDays(-1 * period);
            DateOnly to = date;

            int count = Convert.ToInt32(Math.Truncate(CandleData.Count * percent / 100.0));

            var topTickers = CandleData
                .ToDictionary(k => k.Key, v => GetDeltaPercent(v.Value, from, to))
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .Take(count)
                .Select(x => x.Key)
                .ToList();

            memoryCache.Set(key, topTickers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));

            return topTickers ?? [];
        }

        private static double GetDeltaPercent(List<Candle> candles, DateOnly from, DateOnly to)
        {
            var candlesFromTo = candles.Where(x => x.Date >= from).Where(x => x.Date <= to).ToList();

            if (candlesFromTo is []) return 0.0;

            double firstPrice = candlesFromTo.First().Close;
            double lastPrice = candlesFromTo.Last().Close;

            if (firstPrice == 0.0) return 0.0;
            if (lastPrice == 0.0) return 0.0;

            return (lastPrice - firstPrice) / firstPrice * 100.0;
        }
    }
}
