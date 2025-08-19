using Database.Entities;

namespace RGFinance.FlowFeature
{
    // Alias for base currencies (subset of CurrencyType)
    public enum BaseCurrency
    {
        PLN = CurrencyType.PLN,
        USD = CurrencyType.USD,
        EUR = CurrencyType.EUR
    }
}
