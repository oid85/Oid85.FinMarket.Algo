namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class MonitorResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public List<PositionList> PositionLists { get; set; } = [];
        public List<PortfolioBacktestSeries> Series { get; set; } = [];
    }

    public class PositionList
    {
        public string Ticker { get; set; } = string.Empty;
        public List<PositionItem> PositionItems { get; set; } = [];
    }

    public class PositionItem
    {
        public DateOnly Date { get; set; }
        public string ColorFill { get; set; } = "#FFFFFF";
        public int? Units { get; set; } = null;
    }

    public class PortfolioBacktestSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<PortfolioRebalanceSeriesItem> Data { get; set; } = [];
    }

    public class PortfolioRebalanceSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
