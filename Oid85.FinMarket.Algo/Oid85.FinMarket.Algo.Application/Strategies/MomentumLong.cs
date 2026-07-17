using Oid85.FinMarket.Algo.Application.Interfaces.Factories;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    public class MomentumLong(
        IIndicatorFactory indicatorFactory) 
        : Strategy
    {
        public override void Execute()
        {
            // Получаем параметры
            int period = Parameters["Period"];
            int percent = Parameters["Percent"];

            var tickers = CandleData.Keys.ToList();

            for (int i = StabilizationPeriod; i < Candles.Count - 1; i++)
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
                
                // Отрисовка
                DiagramPoints[i].Price = Candles[i].Close;

                if (LastActivePosition is not null)
                    if (LastActivePosition.IsActive)
                    {
                        if (LastActivePosition.IsLong) DiagramPoints[i].PositionDirection = 1;
                        if (LastActivePosition.IsShort) DiagramPoints[i].PositionDirection = -1;
                    }
            }
        }

        private List<string> GetTopTickers(DateOnly date, int period, int percent)
        {
            DateOnly from = date.AddDays(-1 * period);
            DateOnly to = date;

            int count = Convert.ToInt32(Math.Truncate(CandleData.Count * percent / 100.0));

            var topTickers = CandleData
                .ToDictionary(k => k.Key, v => GetDelta(v.Value, from, to))
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .Take(count)
                .Select(x => x.Key)
                .ToList();

            return topTickers ?? [];
        }

        private static double GetDelta(List<Candle> candles, DateOnly from, DateOnly to)
        {
            double delta = 0;

            return delta;
        }
    }
}
