using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    public class MomentumLong(
        IDataService dataService)
        : Strategy
    {
        public new string Name { get; set; } = nameof(MomentumLong);

        public new string Description { get; set; } = "Momentum. Только лонг";

        public new List<StrategyParameter> StrategyParameters { get; set; } =
            [
                new () { Name = "Period", Def = 10, Min = 10, Max = 100, Step = 10 },
                new () { Name = "Percent", Def = 10, Min = 10, Max = 50, Step = 10 }
            ];

        public override void Execute()
        {
            // Получаем параметры
            int period = Parameters["Period"];
            int percent = Parameters["Percent"];

            for (int i = StabilizationPeriod; i < Candles.Count - 1; i++)
            {
                // Торгуем только в начале месяца
                bool isBalancing = Candles[i].Date.Month == Candles[i - 1].Date.Month + 1;

                if (!isBalancing)
                    continue;

                var topTickers = dataService.GetMomentumTopTickers(CandleData, Candles[i].Date, period, percent);

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

                if (LastActivePosition is not null && LastActivePosition.IsLong)
                    DiagramPoints[i].LongPositionIndicator = Candles[i].Close;
            }
        }
    }
}
