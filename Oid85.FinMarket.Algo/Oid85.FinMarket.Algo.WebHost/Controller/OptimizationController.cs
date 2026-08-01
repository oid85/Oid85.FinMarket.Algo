using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core;
using Oid85.FinMarket.Algo.Core.Responses;
using Oid85.FinMarket.Algo.WebHost.Controller.Base;

namespace Oid85.FinMarket.Algo.WebHost.Controller;

/// <summary>
/// Оптимизация
/// </summary>
[Route("api/optimization")]
[ApiController]
public class OptimizationController(
    IAlgoService algoService)
    : BaseController
{
    /// <summary>
    /// Оптимизация всех портфелей
    /// </summary>
    [HttpPost("portfolio")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationAsync() =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(new() { PortfolioName = string.Empty }),
            result => new BaseResponse<OptimizationResponse> { Result = result });

    /// <summary>
    /// Оптимизация Trend_Life
    /// </summary>
    [HttpPost("portfolio/trend-life")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationTrendLifeAsync() =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(new() { PortfolioName = "Trend_Life" }),
            result => new BaseResponse<OptimizationResponse> { Result = result });

    /// <summary>
    /// Оптимизация Momentum_Life
    /// </summary>
    [HttpPost("portfolio/momentum-life")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationMomentumLifeAsync() =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(new() { PortfolioName = "Momentum_Life" }),
            result => new BaseResponse<OptimizationResponse> { Result = result });

    /// <summary>
    /// Оптимизация Test
    /// </summary>
    [HttpPost("portfolio/test")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationHmaAsync() =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(new() { PortfolioName = "Test" }),
            result => new BaseResponse<OptimizationResponse> { Result = result });

    /// <summary>
    /// Оптимизация Momentum
    /// </summary>
    [HttpPost("portfolio/momentum")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationMomentumAsync() =>
        GetResponseAsync(
            () => algoService.OptimizationAsync(new() { PortfolioName = "Momentum" }),
            result => new BaseResponse<OptimizationResponse> { Result = result });
}