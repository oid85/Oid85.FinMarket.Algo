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
    /// Мониторинг
    /// </summary>
    [HttpPost("portfolio/monitor")]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> MonitorAsync(
        [FromBody] MonitorRequest request) =>
        GetResponseAsync(
            () => algoService.MonitorAsync(request),
            result => new BaseResponse<MonitorResponse> { Result = result });

    /// <summary>
    /// Список портфелей
    /// </summary>
    [HttpPost("portfolio/list")]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioListAsync(
        [FromBody] PortfolioListRequest request) =>
        GetResponseAsync(
            () => algoService.PortfolioListAsync(request),
            result => new BaseResponse<PortfolioListResponse> { Result = result });
}