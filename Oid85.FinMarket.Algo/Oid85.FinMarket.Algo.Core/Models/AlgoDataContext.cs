namespace Oid85.FinMarket.Algo.Core.Models
{
    public class AlgoDataContext
    {
        public Dictionary<string, List<Candle>> CandleData { get; set; } = [];
        public Dictionary<string, double> PeriodDeltaPriceData { get; set; } = [];
    }
}
