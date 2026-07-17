namespace Oid85.FinMarket.Algo.Core.Models
{
    /// <summary>
    /// Строка - значение
    /// </summary>
    public class StringValue<T>
    {
        public string Date { get; set; }
        public T Value { get; set; }
    }
}
