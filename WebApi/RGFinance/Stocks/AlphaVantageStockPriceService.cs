using System.Text.Json;

namespace RGFinance.StocksFeature
{
    public class AlphaVantageStockPriceService : IStockPriceService
    {
        private readonly string? _apiKey;
        private readonly HttpClient _httpClient;

        public AlphaVantageStockPriceService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["AlphaVantageApiKey"] ?? Environment.GetEnvironmentVariable("ALPHA_VANTAGE_APIKEY");
            _httpClient = httpClient;

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Alpha Vantage API key is not configured. Set ALPHA_VANTAGE_APIKEY environment variable or add AlphaVantageApiKey to appsettings.json");
            }
        }

        public async Task<decimal> GetStockPrice(string ticker)
        {
            try
            {
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={_apiKey}";
                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                // Response structure: { "Global Quote": { "05. price": "123.45" } }
                if (root.TryGetProperty("Global Quote", out var globalQuoteElement))
                {
                    if (globalQuoteElement.TryGetProperty("05. price", out var priceElement))
                    {
                        var priceString = priceElement.GetString();
                        if (decimal.TryParse(priceString, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var price))
                        {
                            return price;
                        }
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
