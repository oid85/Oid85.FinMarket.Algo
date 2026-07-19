using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Helpers
{
    public class MonitorHelper
    {
        public static List<(string Ticker, List<DateWeight> WeightData)> GetPositionWeightData(
            List<StrategyExecuteResult> strategyExecuteResults,
            List<string> tickers,
            List<DateOnly> dates)
        {
            var result = new List<(string Ticker, List<DateWeight> WeightData)>();

            foreach (var ticker in tickers)
            {
                var data = strategyExecuteResults
                    .Where(x => x.Ticker == ticker)
                    .Select(x => FillEmptyDates(Map(x.DiagramPoints), dates))
                    .ToList();

                result.Add(new (ticker, [.. Merge(data, dates).Select(x => new DateWeight { Date = x.Date, Weight = x.Value})]));
            }

            return [.. result.OrderBy(x => x.Ticker)];
        }

        public static List<TickerWeight> GetPositionWeightDataByDate(
            List<(string Ticker, List<DateWeight> WeightData)> weightData,
            DateOnly date)
        {
            var result = new List<TickerWeight>();

            foreach (var weightDataItem in weightData)
                result.Add(
                    new()
                    {
                        Ticker = weightDataItem.Ticker,
                        Weight = weightDataItem.WeightData.Find(x => x.Date == date)?.Weight ?? 0
                    });

            return result;
        }

        public static List<DateValue<int>> FillEmptyDates(List<DateValue<int>> dateValues, List<DateOnly> dates)
        {
            var result = new List<DateValue<int>>();

            foreach (var date in dates)
            {
                var dateValue = dateValues.Find(x => x.Date == date);

                if (dateValue is not null)
                    result.Add(dateValue);

                else
                {
                    dateValue = dateValues.FindLast(x => x.Date <= date);

                    if (dateValue is not null)
                        result.Add(new() { Date = date, Value = dateValue.Value });

                    else
                        result.Add(new() { Date = date, Value = 0 });
                }
            }

            return result;
        }

        public static List<DateValue<int>> Merge(List<List<DateValue<int>>> data, List<DateOnly> dates)
        {
            var result = new List<DateValue<int>>();

            var combineData = data.SelectMany(x => x).ToList();

            foreach (var date in dates)
                result.Add(
                    new()
                    {
                        Date = date,
                        Value = combineData.Where(x => x.Date == date).Sum(x => x.Value)
                    });

            return result;
        }

        public static List<DateValue<int>> Map(List<DiagramPoint> diagramPoints)
        {
            var result = new List<DateValue<int>>();

            foreach (var diagramPoint in diagramPoints)
            {
                var dateValue = new DateValue<int> { Date = diagramPoint.Date, Value = 0 };

                if (diagramPoint.PositionDirection.HasValue)
                    if (diagramPoint.PositionDirection.Value == 1)
                        dateValue.Value = 1;

                result.Add(dateValue);
            }

            return [.. result.OrderBy(x => x.Date)];
        }
    }
}
