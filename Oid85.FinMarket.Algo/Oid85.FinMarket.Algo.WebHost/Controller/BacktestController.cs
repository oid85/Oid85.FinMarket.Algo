using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Core;
using Oid85.FinMarket.Algo.Core.Requests;
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
    /// Результат бэктеста
    /// </summary>
    [HttpPost("portfolio/result/list")]
    [ProducesResponseType(typeof(BaseResponse<GetBacktestResultListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetBacktestResultListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetBacktestResultListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetBacktestResultListAsync(
        [FromBody] GetBacktestResultListRequest request) =>
        GetResponseAsync(
            () => algoService.GetBacktestResultListAsync(request),
            result => new BaseResponse<GetBacktestResultListResponse> { Result = result });
}