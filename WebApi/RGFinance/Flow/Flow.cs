using Database.Entities;

namespace RGFinance.FlowFeature
{
    public class Flow
    {
        public decimal BigSum { get; set; }
        public List<State> States { get; set; } = new List<State>();
        public List<Profit> Profits { get; set; } = new List<Profit> { };
        public List<Expense> Expenses { get; set; } = new List<Expense> { };
    }
}
