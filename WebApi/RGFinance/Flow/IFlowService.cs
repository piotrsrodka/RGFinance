using Database.Entities;

namespace RGFinance.FlowFeature
{
    public interface IFlowService
    {
        Task<Flow> GetFlowAsync(int id);
        Task<int> AddOrUpdateState(State state);
        Task<int> AddOrUpdateProfit(Profit profit);
        Task<int> AddOrUpdateExpense(Expense expense);
        Task<Forex> GetForex();
        Task DeleteProfit(int profitId);
        Task DeleteState(int id);
        Task DeleteExpense(int id);
    }
}