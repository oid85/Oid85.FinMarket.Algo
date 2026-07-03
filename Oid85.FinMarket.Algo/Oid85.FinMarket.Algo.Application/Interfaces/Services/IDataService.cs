using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IDataService
    {
        Task<List<Candle>> GetCandlesAsync(string ticker);
    }
}
