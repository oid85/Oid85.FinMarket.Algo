using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core;
using Oid85.FinMarket.Algo.Core.Requests;
using Oid85.FinMarket.Algo.Core.Responses;
using Oid85.FinMarket.Algo.WebHost.Controller.Base;

namespace Oid85.FinMarket.Algo.WebHost.Controller;

/// <summary>
/// Алго
/// </summary>
[Route("api/algo")]
[ApiController]
public class AlgoController(
    IAlgoService algoService)
    : BaseController
{
    /// <summary>
    /// Стратегия
    /// </summary>
    [HttpPost("strategy")]
    [ProducesResponseType(typeof(BaseResponse<GetStrategyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetStrategyResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetStrategyResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetStrategyAsync(
        [FromBody] GetStrategyRequest request) =>
        GetResponseAsync(
            () => algoService.GetStrategyAsync(request),
            result => new BaseResponse<GetStrategyResponse> { Result = result });
}