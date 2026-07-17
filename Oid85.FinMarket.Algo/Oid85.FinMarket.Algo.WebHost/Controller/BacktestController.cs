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
    /// Бэктест Trend
    /// </summary>
    [HttpPost("portfolio/trend")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestTrendAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "Trend" }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест UltimateSmoother
    /// </summary>
    [HttpPost("portfolio/ultimate-smoother")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestUltimateSmootherAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "UltimateSmoother" }),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Бэктест Supertrend
    /// </summary>
    [HttpPost("portfolio/supertrend")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestSupertrendAsync() =>
        GetResponseAsync(
            () => algoService.BacktestAsync(new() { PortfolioName = "Supertrend" }),
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
}