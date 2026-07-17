namespace Oid85.FinMarket.Algo.Core.Models
{
    public class PortfolioData
    {
        /// <summary>
        /// Капитал
        /// </summary>
        public List<DateValue<double>> EqiutyCurve { get; set; } = [];

        /// <summary>
        /// Просадка
        /// </summary>
        public List<DateValue<double>> DrawdownCurve { get; set; } = [];

        /// <summary>
        /// Денежные средства и эквиваленты
        /// </summary>
        public List<DateValue<double>> MoneyCurve { get; set; } = [];
    }
}
