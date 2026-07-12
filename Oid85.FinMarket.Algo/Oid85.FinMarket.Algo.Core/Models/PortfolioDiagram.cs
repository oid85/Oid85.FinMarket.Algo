namespace Oid85.FinMarket.Algo.Core.Models
{
    public class PortfolioDiagram
    {
        /// <summary>
        /// Капитал
        /// </summary>
        public List<DateValue<double>> EqiutyCurve { get; set; } = [];

        /// <summary>
        /// Просадка
        /// </summary>
        public List<DateValue<double>> DrawdownCurve { get; set; } = [];
    }
}
