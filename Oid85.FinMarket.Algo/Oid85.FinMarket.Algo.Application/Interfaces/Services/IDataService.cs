using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IDataService
    {
        Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers);
        Task<Dictionary<string, Instrument>> GetInstrumentDataAsync(List<string> tickers);
        double? GetPrice(string ticker, DateOnly date);
    }
}
