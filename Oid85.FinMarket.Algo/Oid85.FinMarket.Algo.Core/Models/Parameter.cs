using Oid85.FinMarket.Algo.Core.Models.Base;

namespace Oid85.FinMarket.Algo.Core.Models
{
    /// <summary>
    /// Параметр
    /// </summary>
    public class Parameter : BaseModel
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}
