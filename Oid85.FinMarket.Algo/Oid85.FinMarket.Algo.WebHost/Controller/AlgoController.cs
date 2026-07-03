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
    [HttpPost("strategy/backtest")]
    [ProducesResponseType(typeof(BaseResponse<StrategyBacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<StrategyBacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<StrategyBacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> StrategyBacktestAsync(
        [FromBody] StrategyBacktestRequest request) =>
        GetResponseAsync(
            () => algoService.StrategyBacktestAsync(request),
            result => new BaseResponse<StrategyBacktestResponse> { Result = result });

    /// <summary>
    /// Оптимизация
    /// </summary>
    [HttpPost("strategy/optimization")]
    [ProducesResponseType(typeof(BaseResponse<StrategyOptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<StrategyOptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<StrategyOptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> StrategyOptimizationAsync(
        [FromBody] StrategyOptimizationRequest request) =>
        GetResponseAsync(
            () => algoService.StrategyOptimizationAsync(request),
            result => new BaseResponse<StrategyOptimizationResponse> { Result = result });
}