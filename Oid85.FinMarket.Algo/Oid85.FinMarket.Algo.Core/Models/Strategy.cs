namespace Oid85.FinMarket.Algo.Core.Models;

public class Strategy
{
    public Guid StrategyId { get; set; }
    
    public double StartMoney { get; set; }

    public double EndMoney { get; set; }
    
    public string Ticker { get; set; } = string.Empty;
    
    public bool IsFuture { get; set; }
    
    public double BasicAssetSize { get; set; } = 1.0;
    
    public double Leverage { get; set; } = 1.0;
    
    public string Timeframe { get; set; } = string.Empty;
    
    public string StrategyDescription { get; set; } = string.Empty;
    
    public string StrategyName { get; set; } = string.Empty;
    
    public DateOnly StartDate => DateOnly.FromDateTime(Candles.First().DateTime);
    
    public DateOnly EndDate => DateOnly.FromDateTime(Candles.Last().DateTime);
    
    public Dictionary<string, int> Parameters { get; set; } = [];

    public int StabilizationPeriod { get; set; } = 1;
    
    public List<Candle> Candles { get; set; } = [];

    public List<double> OpenPrices => [.. Candles.Select(x => x.Open)];
    
    public List<double> ClosePrices => [.. Candles.Select(x => x.Close)];
    
    public List<double> HighPrices => [.. Candles.Select(x => x.High)];
    
    public List<double> LowPrices => [.. Candles.Select(x => x.Low)];

    public List<DiagramPoint> DiagramPoints { get; set; } = [];
    
    public bool SignalLong { get; set; }
    
    public bool SignalShort { get; set; }

    public bool FilterLong { get; set; } = true;

    public bool FilterShort { get; set; } = true;
    
    public bool SignalCloseLong { get; set; }
    
    public bool SignalCloseShort { get; set; }
    
    public List<StopLimit?> StopLimits { get; set; } = [];

    public List<Position> Positions { get; set; } = [];

    public int GetPositionSize(double orderPrice)
    {
        if (EndMoney <= orderPrice)
            return 0;
        
        if (IsFuture)
            return orderPrice == 0.0 || BasicAssetSize == 0.0 ? 0 : Convert.ToInt32(EndMoney / (orderPrice * BasicAssetSize) * Leverage);
        
        return orderPrice == 0.0 ? 0 : Convert.ToInt32(EndMoney / orderPrice * Leverage);        
    }

    public Position? LastActivePosition {
        get
        {
            if (LastPosition is null)
                return null;
            
            if (!LastPosition.IsActive)
                return null;

            return LastPosition;
        }
    }

    public Position? LastPosition => Positions.Count == 0 ? null : Positions.Last();
    
    public int CurrentPosition
    {
        get
        {
            if (LastActivePosition == null)
                return 0;

            if (LastActivePosition.IsLong)
                return Math.Abs(LastActivePosition.Quantity);

            if (LastActivePosition.IsShort)
                return -1 * Math.Abs(LastActivePosition.Quantity);

            return 0;
        }
    }

    public double CurrentPositionCost
    {
        get
        {
            if (LastActivePosition == null)
                return 0.0;

            if (LastActivePosition.IsLong)
                return LastActivePosition.Cost;

            if (LastActivePosition.IsShort)
                return -1 * Math.Abs(LastActivePosition.Cost);

            return 0.0;
        }
    }
    
    public void BuyAtPrice(int quantity, double price, int candleIndex) =>
        AddTrade(new Trade
        {
            CandleIndex = candleIndex,
            Quantity = Math.Abs(quantity),
            Price = price,
            DateTime = Candles[candleIndex].DateTime
        });

    public void SellAtPrice(int quantity, double price, int candleIndex) =>
        AddTrade(new Trade
        {
            CandleIndex = candleIndex,
            Quantity = -1 * Math.Abs(quantity),
            Price = price,
            DateTime = Candles[candleIndex].DateTime
        });

    private void AddTrade(Trade trade)
    {
        if (trade.Quantity == 0)
            return;
        
        if (LastActivePosition is null)
            Positions.Add(new Position
            {
                Ticker = Ticker,
                EntryPrice = trade.Price,
                EntryDateTime = trade.DateTime,
                EntryCandleIndex = trade.CandleIndex,
                IsActive = true,
                IsLong = trade.Quantity > 0,
                IsShort = trade.Quantity < 0,
                Quantity = trade.Quantity,
                Cost = trade.Quantity * trade.Price
            });

        else
        {
            int count = Positions.Count;

            Positions[count - 1].ExitPrice = trade.Price;
            Positions[count - 1].ExitDateTime = trade.DateTime;
            Positions[count - 1].ExitCandleIndex = trade.CandleIndex;
            Positions[count - 1].IsActive = false;

            double profit = 0.0;
            
            if (Positions[count - 1].IsLong)
                profit = Math.Abs(Positions[count - 1].Quantity) * (Positions[count - 1].ExitPrice - Positions[count - 1].EntryPrice);
            
            if (Positions[count - 1].IsShort)
                profit = Math.Abs(Positions[count - 1].Quantity) * (Positions[count - 1].EntryPrice - Positions[count - 1].ExitPrice);            
            
            Positions[count - 1].Profit = profit;
            Positions[count - 1].ProfitPercent = profit / EndMoney * 100.0;
            
            var totalProfit = Positions.Sum(x => x.Profit);
            Positions[count - 1].TotalProfit = totalProfit;
            Positions[count - 1].TotalProfitPct = totalProfit / EndMoney * 100.0;
            
            EndMoney += profit;
            
            EqiutyCurve.TryAdd(Positions[count - 1].ExitDateTime, Positions[count - 1].TotalProfit);
            
            double drawdown;
            
            if (count < 2)
                drawdown = 0.0;

            else
            {
                var maxTotalProfit = Positions.Take(count - 1).Max(x => x.TotalProfit);

                drawdown = Positions[count - 1].TotalProfit >= maxTotalProfit
                    ? 0.0
                    : maxTotalProfit - Positions[count - 1].TotalProfit;
            }
            
            DrawdownCurve.TryAdd(Positions[count - 1].ExitDateTime, drawdown);
        }
    }

    public void CloseAtStop(Position position, double stopPrice, int candleIndex)
    {
        if (StopLimits.Count != Candles.Count)
        {
            StopLimits.Clear();
            
            for (int i = 0; i < Candles.Count; i++) 
                StopLimits.Add(null);            
        }

        // Если последняя свеча
		if (candleIndex > StopLimits.Count - 1)
			return;
		
		// Пробуем выйти по стопу
		if (LastActivePosition is not null && StopLimits[candleIndex - 1] is not null)		
		{
			if (position.IsLong && Candles[candleIndex - 1].Close <= StopLimits[candleIndex - 1]!.Value.StopPrice)			
				SellAtPrice(position.Quantity, stopPrice, candleIndex);	
			
			else if (position.IsShort && Candles[candleIndex - 1].Close >= StopLimits[candleIndex - 1]!.Value.StopPrice)			
				BuyAtPrice(position.Quantity, stopPrice, candleIndex);				
		}
		
		// Если не вышли, то переставляем стоп
		if (LastActivePosition is not null)
			StopLimits[candleIndex] = new StopLimit { StopPrice = stopPrice, Quantity = position.Quantity };
    }    
    
    public void CloseAtPrice(Position position, double price, int candleIndex)
    {
        // Отправляем команды, если длинная позиция
        if (position.IsLong)
            SellAtPrice(position.Quantity, price, candleIndex);

        // Отправляем команды, если короткая позиция
        else if (position.IsShort)
            BuyAtPrice(position.Quantity, price, candleIndex);
    }

    public Dictionary<DateTime, double> EqiutyCurve { get; set; } = [];

    public Dictionary<DateTime, double> DrawdownCurve  { get; set; } = [];
    
    public double ProfitFactor
    {
        get
        {
            double profits = Positions.Where(x => x.Profit > 0.0).Sum(x => x.Profit);
            double losses = Positions.Where(x => x.Profit < 0.0).Sum(x => x.Profit);

            if (losses == 0.0)
                return double.PositiveInfinity;
            
            return profits / Math.Abs(losses);
        }
    }

    public double RecoveryFactor => MaxDrawdown == 0.0 ? double.PositiveInfinity : NetProfit / MaxDrawdown;

    public double NetProfit => LastPosition?.TotalProfit ?? 0.0;
    
    public double AverageNetProfit => Positions.Count == 0 ? 0.0 : Positions.Select(x => x.Profit).Average();

    public double AverageNetProfitPercent => Positions.Count == 0 ? 0.0 : Positions.Select(x => x.ProfitPercent).Average();

    public double Drawdown  => LastPosition is null ? 0.0 : Positions.Max(x => x.TotalProfit) - LastPosition.TotalProfit;

    public double MaxDrawdown  => DrawdownCurve.Count == 0 ? 0.0 : Math.Abs(DrawdownCurve.Max(x => x.Value));

    public double MaxDrawdownPercent => EqiutyCurve.Count == 0 ? 0.0 : Math.Abs(MaxDrawdown / EqiutyCurve.Max(x => x.Value) * 100.0);

    public int NumberPositions => Positions.Count;

    public int WinningPositions => Positions.Count == 0 ? 0 : Positions.Count(x => x.Profit > 0.0);

    public double WinningTradesPercent => NumberPositions == 0.0 ? 0.0 : Convert.ToDouble(WinningPositions) / Convert.ToDouble(NumberPositions) * 100.0;    
    
    public double TotalReturn => EndMoney > StartMoney ? (EndMoney - StartMoney) / StartMoney * 100.0 : 0.0;
    
    public double AnnualYieldReturn => EndMoney > StartMoney ? TotalReturn / ((EndDate.DayNumber - StartDate.DayNumber) / 365.0): 0.0;
    
    public virtual void Execute()
    {

    }

    public void InitForParameterSet(Dictionary<string, int> parameterSet, int stabilizationPeriod, double startMoney, double endMoney)
    {
        Parameters = parameterSet;
        StopLimits.Clear();
        Positions.Clear();
        EqiutyCurve.Clear();
        DrawdownCurve.Clear();
        StabilizationPeriod = stabilizationPeriod;
        StartMoney = startMoney;
        EndMoney = endMoney;

        DiagramPoints.Clear();
        for (int i = 0; i < Candles.Count; i++)
            DiagramPoints.Add(new()
            {
                Index = Candles[i].Index,
                Date = DateOnly.FromDateTime(Candles[i].DateTime),
            });
    }
}