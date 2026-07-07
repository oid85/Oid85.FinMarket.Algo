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
    [HttpPost("portfolio/backtest")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestAsync(
        [FromBody] BacktestRequest request) =>
        GetResponseAsync(
            () => algoService.BacktestAsync(request),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Оптимизация
    /// </summary>
    [HttpPost("portfolio/optimization")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationAsync(
        [FromBody] OptimizationRequest request) =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(request),
            result => new BaseResponse<OptimizationResponse> { Result = result });

    /// <summary>
    /// Эмуляция портфеля
    /// </summary>
    [HttpPost("portfolio/emulate")]
    [ProducesResponseType(typeof(BaseResponse<EmulateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EmulateResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EmulateResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EmulateAsync(
        [FromBody] EmulateRequest request) =>
        GetResponseAsync(
            () => algoService.EmulateAsync(request),
            result => new BaseResponse<EmulateResponse> { Result = result });
}