namespace RGFinance.StocksFeature
{
    public interface IStockPriceService
    {
        Task<decimal> GetStockPrice(string ticker);
    }
}
