using Database.Entities;

namespace RGFinance.FlowFeature
{
    public interface IFlowService
    {
        Task<Flow> GetFlowAsync(BaseCurrency baseCurrency = BaseCurrency.PLN);
        Task<int> AddOrUpdateAsset(Asset asset);
        Task<int> AddOrUpdateProfit(Profit profit);
        Task<int> AddOrUpdateExpense(Expense expense);
        Task DeleteProfit(int profitId);
        Task DeleteAsset(int id);
        Task DeleteExpense(int id);
    }
}
