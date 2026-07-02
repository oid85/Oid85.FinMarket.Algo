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
    /// Бэктест
    /// </summary>
    [HttpPost("backtest")]
    [ProducesResponseType(typeof(BaseResponse<CreateBacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateBacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CreateBacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CreateBacktestAsync(
        [FromBody] CreateBacktestRequest request) =>
        GetResponseAsync(
            () => algoService.CreateBacktestAsync(request),
            result => new BaseResponse<CreateBacktestResponse> { Result = result });
}