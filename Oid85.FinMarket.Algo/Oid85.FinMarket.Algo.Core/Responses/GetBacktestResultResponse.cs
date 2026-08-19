namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class GetBacktestResultResponse
    {
        public BacktestResultSeries Price { get; set; } = new();
        public BacktestResultSeries Equity { get; set; } = new();
        public BacktestResultSeries Drawdown { get; set; } = new();
    }

    public class BacktestResultSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<PortfolioBacktestSeriesItem> Data { get; set; } = [];
    }

    public class BacktestResultSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
