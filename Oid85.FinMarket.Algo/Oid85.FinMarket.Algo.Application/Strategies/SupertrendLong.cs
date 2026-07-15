using Oid85.FinMarket.Algo.Application.Interfaces.Factories;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    public class SupertrendLong(
        IIndicatorFactory indicatorFactory) 
        : Strategy
    {
        public override void Execute()
        {
            // Получаем параметры
            int period = Parameters["Period"];
            double multiplier = Parameters["Multiplier"] / 10.0;
            
            // Расчет индикаторов
            List<double> supertrend = indicatorFactory.Supertrend(Candles, period, multiplier);

            for (int i = StabilizationPeriod; i < Candles.Count - 1; i++)
            {
                // Правило входа
                SignalLong = Candles[i].Close > supertrend[i];
                
                // Правило выхода
                SignalCloseLong = Candles[i].Close < supertrend[i];
                
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
                DiagramPoints[i].Indicator = supertrend[i];
                DiagramPoints[i].Price = Candles[i].Close;

                if (LastActivePosition is not null)
                    if (LastActivePosition.IsActive)
                    {
                        if (LastActivePosition.IsLong) DiagramPoints[i].PositionDirection = 1;
                        if (LastActivePosition.IsShort) DiagramPoints[i].PositionDirection = -1;
                    }
            }
        }
    }
}
