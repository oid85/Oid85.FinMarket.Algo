using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core;
using Oid85.FinMarket.Algo.Core.Responses;
using Oid85.FinMarket.Algo.WebHost.Controller.Base;

namespace Oid85.FinMarket.Algo.WebHost.Controller;

/// <summary>
/// Бектест
/// </summary>
[Route("api/backtest")]
[ApiController]
public class BacktestController(
    IAlgoService algoService)
    : BaseController
{
    /// <summary>
    /// Бэктест всех портфелей
    /// </summary>
    [HttpPost("portfolio")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = string.Empty }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест Trend_Life
    /// </summary>
    [HttpPost("portfolio/trend-life")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestTrendLifeAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "Trend_Life" }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест Momentum_Life
    /// </summary>
    [HttpPost("portfolio/momentum-life")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestMomentumLifeAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "Momentum_Life" }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест Momentum
    /// </summary>
    [HttpPost("portfolio/momentum")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestMomentumAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "Momentum" }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест NormalizedMomentum
    /// </summary>
    [HttpPost("portfolio/normalized-momentum")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestNormalizedMomentumAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "NormalizedMomentum" }),
            result => new BaseResponse<BacktestResponse> { Result = result });
}