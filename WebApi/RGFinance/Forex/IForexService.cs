using Database.Entities;

namespace RGFinance.FlowFeature
{
    public interface IForexService
    {
        Task<Forex> GetForex();
        Task<Forex> GetForexFromApi();
    }
}
