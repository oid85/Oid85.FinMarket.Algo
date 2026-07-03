namespace Oid85.FinMarket.Algo.Core.Models;

public class StrategyExecuteResult
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Начало периода
    /// </summary>
    public DateOnly StartDate { get; set; }
    
    /// <summary>
    /// Конец периода
    /// </summary>
    public DateOnly EndDate { get; set; }
    
    /// <summary>
    /// Тикер инструмента
    /// </summary>
    public string Ticker { get; set; } = string.Empty;
    
    /// <summary>
    /// Таймфрейм
    /// </summary>
    public string Timeframe { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор стратегии
    /// </summary>
    public Guid StrategyId { get; set; }
    
    /// <summary>
    /// Описание стратегии
    /// </summary>
    public string StrategyDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Наименование стратегии
    /// </summary>
    public string StrategyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Параметры стратегии
    /// </summary>
    public string StrategyParams { get; set; } = string.Empty;
    
    /// <summary>
    /// Параметры стратегии (хэш)
    /// </summary>
    public string StrategyParamsHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Количество сделок
    /// </summary>
    public int NumberPositions { get; set; }
    
    /// <summary>
    /// Текущая позиция
    /// </summary>
    public int CurrentPosition { get; set; }    
    
    /// <summary>
    /// Текущая позиция (стоимость)
    /// </summary>
    public double CurrentPositionCost { get; set; }
    
    /// <summary>
    /// Profit Factor
    /// </summary>
    public double ProfitFactor { get; set; }
    
    /// <summary>
    /// Recovery Factor
    /// </summary>
    public double RecoveryFactor { get; set; }
    
    /// <summary>
    /// Net Profit
    /// </summary>
    public double NetProfit { get; set; }
    
    /// <summary>
    /// Average Profit
    /// </summary>
    public double AverageProfit { get; set; }
    
    /// <summary>
    /// Average Profit Percent
    /// </summary>
    public double AverageProfitPercent { get; set; }
    
    /// <summary>
    /// Drawdown
    /// </summary>
    public double Drawdown { get; set; }
    
    /// <summary>
    /// Max Drawdown
    /// </summary>
    public double MaxDrawdown { get; set; }
    
    /// <summary>
    /// MaxDrawdownPercent
    /// </summary>
    public double MaxDrawdownPercent { get; set; }
    
    /// <summary>
    /// Winning Positions
    /// </summary>
    public int WinningPositions { get; set; }
    
    /// <summary>
    /// Winning Trades Percent
    /// </summary>
    public double WinningTradesPercent { get; set; }
    
    /// <summary>
    /// Start Money
    /// </summary>
    public double StartMoney { get; set; }
    
    /// <summary>
    /// End Money
    /// </summary>
    public double EndMoney { get; set; }
    
    /// <summary>
    /// Доходность всего, %
    /// </summary>
    public double TotalReturn { get; set; }
    
    /// <summary>
    /// Доходность годовая, %;
    /// </summary>
    public double AnnualYieldReturn { get; set; }
}