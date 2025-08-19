namespace RGFinance.FlowFeature
{
    public static class CurrencyExtensions
    {
        public static string ToStringValue(this CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.PLN => "PLN",
                CurrencyType.USD => "USD", 
                CurrencyType.EUR => "EUR",
                CurrencyType.GOZ => "GOZ",
                _ => throw new ArgumentException($"Unknown currency: {currency}")
            };
        }

        public static CurrencyType ToCurrencyType(this string currencyString)
        {
            return currencyString.ToUpper() switch
            {
                "PLN" => CurrencyType.PLN,
                "USD" => CurrencyType.USD,
                "EUR" => CurrencyType.EUR, 
                "GOZ" => CurrencyType.GOZ,
                _ => throw new ArgumentException($"Unknown currency string: {currencyString}")
            };
        }

        public static bool IsBaseCurrency(this CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.PLN => true,
                CurrencyType.USD => true,
                CurrencyType.EUR => true,
                CurrencyType.GOZ => false,  // Gold is not a base currency
                _ => false
            };
        }
    }
}