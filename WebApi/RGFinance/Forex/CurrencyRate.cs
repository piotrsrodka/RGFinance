namespace RGFinance.FlowFeature
{
    public class CurrencyRate
    {
        public string Currency { get; set; } = CurrencyType.PLN.ToStringValue();
        public decimal Rate { get; set; }
    }
}
