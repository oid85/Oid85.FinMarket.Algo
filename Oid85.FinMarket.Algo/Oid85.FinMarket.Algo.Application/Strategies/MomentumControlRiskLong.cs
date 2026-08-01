using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    public class MomentumControlRiskLong(
        IDataService dataService) 
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
    }
}
