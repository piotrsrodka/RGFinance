namespace RGFinance.StocksFeature
{
    public class StockPriceServiceFactory : IStockPriceServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StockPriceServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStockPriceService GetService(string ticker)
        {
            // Determine provider based on ticker suffix
            // European stocks usually have exchange suffix like .DE, .L, .PA, etc.
            if (ticker.Contains('.'))
            {
                // European stock - use Yahoo Finance (no API key required, no rate limit)
                return _serviceProvider.GetRequiredService<YahooFinanceStockPriceService>();
            }
            else
            {
                // US stock - use Polygon.io
                return _serviceProvider.GetRequiredService<PolygonStockPriceService>();
            }
        }
    }
}
