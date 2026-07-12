namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class MonitorResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public List<PositionList> PositionLists { get; set; } = [];
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
}
