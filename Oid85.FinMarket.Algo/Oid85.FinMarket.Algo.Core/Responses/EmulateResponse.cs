namespace Oid85.FinMarket.Algo.Core.Responses
{
    public class EmulateResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public List<PositionList> PositionLists { get; set; } = [];
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
    }
}
