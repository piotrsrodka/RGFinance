using Database.Entities;

namespace RGFinance.FlowFeature
{
    public class Flow
    {
        public decimal BigSum { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
        public List<Profit> Profits { get; set; } = new List<Profit> { };
        public List<Expense> Expenses { get; set; } = new List<Expense> { };
    }
}
