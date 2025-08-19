using Database.Entities;

namespace RGFinance.FlowFeature
{
    public class CurrencyRate
    {
        public string Currency { get; set; } = CurrencyType.PLN.ToString();
        public decimal Rate { get; set; }
    }
}
