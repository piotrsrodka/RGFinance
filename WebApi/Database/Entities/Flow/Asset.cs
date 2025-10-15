using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class Asset : ValueObject
    {
        public decimal Interest { get; set; } // Odsetki roczne
        public Rate InterestRate { get; set; } = Rate.Yearly;
        public AssetType AssetType { get; set; } = AssetType.Undefined;
        public string? Ticker { get; set; } // Stock ticker symbol (e.g., "EOSE" for EOS Energy)

        [NotMapped]
        public bool HasTicker => AssetType == AssetType.Stocks && !string.IsNullOrWhiteSpace(this.Ticker);
    }
}
