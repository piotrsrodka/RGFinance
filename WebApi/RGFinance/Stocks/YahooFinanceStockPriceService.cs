using System.Text.Json;

namespace RGFinance.StocksFeature
{
    public class YahooFinanceStockPriceService : IStockPriceService
    {
        private readonly HttpClient _httpClient;

        public YahooFinanceStockPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetStockPrice(string ticker)
        {
            try
            {
                // Yahoo Finance requires specific ticker format for European stocks
                // Already comes in correct format like QUTM.DE
                var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}";
                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                // Response structure: { "chart": { "result": [{ "meta": { "regularMarketPrice": 123.45 } }] } }
                if (root.TryGetProperty("chart", out var chartElement) &&
                    chartElement.TryGetProperty("result", out var resultElement) &&
                    resultElement.ValueKind == JsonValueKind.Array &&
                    resultElement.GetArrayLength() > 0)
                {
                    var firstResult = resultElement[0];

                    if (firstResult.TryGetProperty("meta", out var metaElement) &&
                        metaElement.TryGetProperty("regularMarketPrice", out var priceElement))
                    {
                        return priceElement.GetDecimal();
                    }
                }

                throw new InvalidOperationException($"Could not extract price for ticker {ticker}. Response: {response}");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to fetch stock price for {ticker}: {ex.Message}", ex);
            }
        }
    }
}
