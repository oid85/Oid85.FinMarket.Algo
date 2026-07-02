using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;

namespace Oid85.FinMarket.Algo.Application.Interfaces.Services
{
    public interface IAlgoService
    {
        Task<GetStrategyResponse> GetStrategyAsync(GetStrategyRequest request);
    }
}
