using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class GetBacktestResultResponse
    {
        public BacktestResultSeries Price { get; set; } = new();
        public BacktestResultSeries LongPositionCost { get; set; } = new();
        public BacktestResultSeries Equity { get; set; } = new();
        public BacktestResultSeries Drawdown { get; set; } = new();
    }

    public class BacktestResultSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<DateValue<double?>> Data { get; set; } = [];
    }
}
