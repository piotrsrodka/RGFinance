namespace RGFinance.StocksFeature
{
    public interface IStockPriceServiceFactory
    {
        IStockPriceService GetService(string ticker);
    }
}
