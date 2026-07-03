using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class DataService : IDataService
    {
        public Task<List<Candle>> GetCandlesAsync(string ticker)
        {
            throw new NotImplementedException();
        }
    }
}
