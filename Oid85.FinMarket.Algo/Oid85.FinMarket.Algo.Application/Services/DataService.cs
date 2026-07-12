using Oid85.FinMarket.Algo.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Common.KnownConstants;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Services
{
    public class DataService(
        IStorageApiClient storageApiClient) 
        : IDataService
    {
        private Dictionary<string, List<Candle>>? _candleData = null;
        private Dictionary<string, Instrument>? _instrumentData = null;

        public async Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers)
        {
            if (_candleData is not null) return _candleData;

            _candleData = [];

            List<string> tickerList = [.. tickers, KnownTickers.TMON];

            foreach (var ticker in tickerList.Distinct()) 
                _candleData.Add(ticker, await GetCandlesByTickerAsync(ticker));

            return _candleData;
        }

        public async Task<Dictionary<string, Instrument>> GetInstrumentDataAsync(List<string> tickers)
        {
            if (_instrumentData is not null) return _instrumentData;

            _instrumentData = (await storageApiClient.GetInstrumentListAsync(new())).Result.Instruments
                .Where(x => tickers.Contains(x.Ticker))
                .ToDictionary(
                k => k.Ticker,
                v => new Instrument
                {
                    Ticker = v.Ticker,
                    Name = v.Name,
                    Type = v.Type,
                    Lot = v.Lot
                });

            return _instrumentData;
        }

        public double? GetPrice(string ticker, DateOnly date)
        {
            if (_candleData is null) return null;

            var candles = _candleData[ticker];

            if (candles is null) return null;

            var candle = candles.Find(x => x.Date == date);

            return candle?.Close;
        }

        private async Task<List<Candle>> GetCandlesByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var response = await storageApiClient.GetCandleListAsync(
                new()
                {
                    From = from,
                    To = to,
                    Ticker = ticker
                });

            var candles =  response.Result.Candles
                .Select(x =>
                new Candle
                {
                    Open = x.Open,
                    Close = x.Close,
                    Low = x.Low,
                    High = x.High,
                    Volume = x.Volume,
                    Date = x.Date
                })
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i < candles.Count; i++) candles[i].Index = i;

            return candles;
        }
    }
}
