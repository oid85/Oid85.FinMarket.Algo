namespace Oid85.FinMarket.Algo.Core.Configuration
{
    public class AlgoSettings
    {
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
}
