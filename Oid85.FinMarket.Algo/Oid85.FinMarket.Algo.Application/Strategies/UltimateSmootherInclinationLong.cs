using Oid85.FinMarket.Algo.Application.Interfaces.Factories;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Strategies
{
    /// <summary>
    /// Правило входа  - индикатор UltimateSmoother растет в течение 3-х периодов подряд
    /// Правило выхода - индикатор UltimateSmoother падает в течение 3-х периодов подряд
    /// </summary>
    public class UltimateSmootherInclinationLong(
        IIndicatorFactory indicatorFactory)
        : Strategy
    {
        public override void Execute()
        {
            // Получаем параметры
            int period = Parameters["Period"];
            
            // Расчет индикаторов
            var us = indicatorFactory.UltimateSmoother(ClosePrices, period);

            for (int i = StabilizationPeriod; i < Candles.Count - 1; i++)
            {
                // Правило входа
                SignalLong = 
                    us[i - 2] > us[i - 3] && 
                    us[i - 1] > us[i - 2] &&
                    us[i] > us[i - 1];

                // Правило выхода
                SignalCloseLong = 
                    us[i - 2] < us[i - 3] &&
                    us[i - 1] < us[i - 2] &&
                    us[i] < us[i - 1];
                
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
                
                // Отрисовка индикаторов
                DiagramPoints[i].Indicator = us[i];
            }
        }
    }
}
