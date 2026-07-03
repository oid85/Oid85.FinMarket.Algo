namespace Oid85.FinMarket.Algo.Core.Models;

public struct Trade
{
    public DateTime DateTime { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int CandleIndex { get; set; }
}