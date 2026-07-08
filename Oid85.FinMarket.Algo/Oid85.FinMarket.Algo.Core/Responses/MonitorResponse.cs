namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class MonitorResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public List<PositionList> PositionLists { get; set; } = [];
        public List<SignalItem> Signals { get; set; } = [];
    }

    public class PositionList
    {
        public string Ticker { get; set; } = string.Empty;
        public List<PositionListItem> PositionListItems { get; set; } = [];
    }

    public class PositionListItem
    {
        public DateOnly Date { get; set; }
        public string? PositionType { get; set; } = null;
        public string ColorFill { get; set; } = "#FFFFFF";
    }

    public class SignalItem
    {
        public string Ticker { get; set; } = string.Empty;
        public int? PositionSize { get; set; } = null;
        public double? PositionCost { get; set; } = null;
    }
}
