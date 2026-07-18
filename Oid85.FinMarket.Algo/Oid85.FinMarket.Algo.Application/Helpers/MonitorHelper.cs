using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Helpers
{
    public class MonitorHelper
    {
        public static List<(string Ticker, List<(DateOnly Date, int Weight)> WeightData)> GetPositionWeightData(
            List<StrategyExecuteResult> strategyExecuteResults,
            List<string> tickers,
            List<DateOnly> dates)
        {
            var result = new List<(string Ticker, List<(DateOnly Date, int Weight)> WeightData)>();



            return result;
        }

        public static List<(string Ticker, int Weight)> GetPositionWeightDataByDate(
            List<(string Ticker, List<(DateOnly Date, int Weight)> WeightData)> weightData,
            DateOnly date)
        {
            var result = new List<(string Ticker, int Weight)>();



            return result;
        }

        public static List<DateValue<int>> FillEmptyDates(List<DateValue<int>> dateValues, List<DateOnly> dates)
        {
            var result = new List<DateValue<int>>();



            return result;
        }

        public static List<DateValue<int>> Merge(List<List<DateValue<int>>> data, List<DateOnly> dates)
        {
            var result = new List<DateValue<int>>();



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
