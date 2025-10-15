using System.Text.Json;

namespace RGFinance.StocksFeature
{
    public class PolygonStockPriceService : IStockPriceService
    {
        private readonly string? _apiKey;
        private readonly HttpClient _httpClient;

        public PolygonStockPriceService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["PolygonApiKey"] ?? Environment.GetEnvironmentVariable("POLYGON_API_KEY");
            _httpClient = httpClient;

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Polygon API key is not configured. Set POLYGON_API_KEY environment variable or add PolygonApiKey to appsettings.json");
            }
        }

        public async Task<decimal> GetStockPrice(string ticker)
        {
            try
            {
                var url = $"https://api.polygon.io/v2/aggs/ticker/{ticker}/prev?apiKey={_apiKey}";
                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.TryGetProperty("results", out var resultsElement) &&
                    resultsElement.ValueKind == JsonValueKind.Array &&
                    resultsElement.GetArrayLength() > 0)
                {
                    var firstResult = resultsElement[0]; // Pierwszy element tablicy

                    if (firstResult.TryGetProperty("c", out var closeElement))
                    {
                        return closeElement.GetDecimal(); // CENA ZAMKNIĘCIA!
                    }
                }

                throw new InvalidOperationException($"Could not extract price for ticker {ticker}");
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to fetch stock price for {ticker}: {ex.Message}", ex);
            }
        }
    }
}
