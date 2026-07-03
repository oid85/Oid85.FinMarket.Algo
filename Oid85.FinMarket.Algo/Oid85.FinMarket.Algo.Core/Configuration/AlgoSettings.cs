namespace Oid85.FinMarket.Algo.Core.Configuration
{
    public class AlgoSettings
    {
        public PeriodSettings PeriodSettings { get; set; } = new();
        public MoneyManagementSettings MoneyManagementSettings { get; set; } = new();
        public StrategyExecuteResultFilterSettings StrategyExecuteResultFilterSettings { get; set; } = new();
        public List<TickerlistSettings> TickerLists { get; set; } = [];
        public List<StrategySettings> Strategies { get; set; } = [];
    }

    public class TickerlistSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tickers { get; set; } = [];
    }

    public class StrategySettings
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool Enable { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TickerList { get; set; } = string.Empty;
        public List<StrategyParameterSettings> StrategyParameters { get; set; } = [];
    }

    public class StrategyParameterSettings
    {
        public string Name { get; set; } = string.Empty;
        public int Def { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Step { get; set; }
    }

    public class PeriodSettings
    {
        public int StabilizationPeriodInCandles { get; set; }
        public int OptimizationWindowInDays { get; set; }
        public int BacktestWindowInDays { get; set; }
        public int BacktestShiftInDays { get; set; }
    }

    public class MoneyManagementSettings
    {
        public double Money { get; set; }
        public double ShareLeverage { get; set; }
    }

    public class StrategyExecuteResultFilterSettings
    {
        public double MinProfitFactor { get; set; }
        public double MinRecoveryFactor { get; set; }
        public double MinWinningTradesPercent { get; set; }
        public double MaxWinningTradesPercent { get; set; }
        public double MinAnnualYieldReturn { get; set; }
        public double MaxDrawdownPercent { get; set; }
    }
}
