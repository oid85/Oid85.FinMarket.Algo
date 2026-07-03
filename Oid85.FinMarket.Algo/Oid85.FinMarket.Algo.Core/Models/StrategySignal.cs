namespace Oid85.FinMarket.Algo.Core.Models;

public class StrategySignal
{
    public StrategySignal()
    {
        
    }
    
    public StrategySignal(string ticker)
    {
        Ticker = ticker;
        CountStrategies = 0;
        CountSignals = 0;
        PercentSignals = 0;
        LastPrice = 0.0;
        PositionCost = 0.0;
        PositionSize = 0;
        PositionPercentPortfolio = 0;
    }
    
    /// <summary>
    /// Тикер инструмента
    /// </summary>
    public string Ticker { get; set; } = string.Empty; 
    
    /// <summary>
    /// Количество сигналов
    /// </summary>
    public int CountSignals { get; set; }
    
    /// <summary>
    /// Количество стратегий
    /// </summary>
    public int CountStrategies { get; set; }    
    
    /// <summary>
    /// Процент сигналов
    /// </summary>
    public double PercentSignals { get; set; }    
    
    /// <summary>
    /// Размер позиции, руб
    /// </summary>
    public double PositionCost { get; set; }   
    
    /// <summary>
    /// Размер позиции, шт
    /// </summary>
    public int PositionSize { get; set; } 
    
    /// <summary>
    /// Размер позиции в процентах от портфеля
    /// </summary>
    public double PositionPercentPortfolio { get; set; }  
    
    /// <summary>
    /// Цена инструмента
    /// </summary>
    public double LastPrice { get; set; } 
}