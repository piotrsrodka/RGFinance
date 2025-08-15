namespace RGFinance.FlowFeature
{
    public class Forex
    {
        public string Time { get; set; }
        public decimal Usd { get; set; }
        public decimal Eur { get; set; }
        public decimal Gold { get; internal set; }
    }

    public class CurrencyRate
    {
        public string Currency { get; set; } = RGFinance.Flow.Currency.PLN;
        public decimal Rate { get; set; }
    }
}