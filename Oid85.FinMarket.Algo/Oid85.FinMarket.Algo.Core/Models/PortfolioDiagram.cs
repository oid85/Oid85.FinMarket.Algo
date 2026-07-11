namespace Oid85.FinMarket.Algo.Core.Models
{
    public class PortfolioDiagram
    {
        /// <summary>
        /// Капитал
        /// </summary>
        public Dictionary<DateOnly, double> EqiutyCurve { get; set; } = [];

        /// <summary>
        /// Просадка
        /// </summary>
        public Dictionary<DateOnly, double> DrawdownCurve { get; set; } = [];
    }
}
