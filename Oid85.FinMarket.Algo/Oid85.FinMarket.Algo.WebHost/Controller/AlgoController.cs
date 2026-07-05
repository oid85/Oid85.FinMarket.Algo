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
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioBacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioBacktestAsync(
        [FromBody] PortfolioBacktestRequest request) =>
        GetResponseAsync(
            () => algoService.PortfolioBacktestAsync(request),
            result => new BaseResponse<PortfolioBacktestResponse> { Result = result });

    /// <summary>
    /// Оптимизация
    /// </summary>
    [HttpPost("portfolio/optimization")]
    [ProducesResponseType(typeof(BaseResponse<PortfolioOptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioOptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioOptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioOptimizationAsync(
        [FromBody] PortfolioOptimizationRequest request) =>
        GetResponseAsync(
            () => algoService.PortfolioOptimizationAsync(request),
            result => new BaseResponse<PortfolioOptimizationResponse> { Result = result });
}