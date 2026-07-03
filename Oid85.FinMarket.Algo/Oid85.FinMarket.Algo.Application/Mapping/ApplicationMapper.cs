using Oid85.FinMarket.Algo.Core.Models;
using Skender.Stock.Indicators;

namespace Oid85.FinMarket.Algo.Application.Mapping;

public static class ApplicationMapper
{
    public static Quote Map(Candle model) =>
        new()
        {
            Open = Convert.ToDecimal(model.Open),
            Close = Convert.ToDecimal(model.Close),
            High = Convert.ToDecimal(model.High),
            Low = Convert.ToDecimal(model.Low),
            Date = model.DateTime
        };
}