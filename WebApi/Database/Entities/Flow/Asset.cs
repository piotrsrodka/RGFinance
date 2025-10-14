namespace Database.Entities
{
    public class Asset : ValueObject
    {
        public decimal Interest { get; set; } // Odsetki roczne
        public Rate InterestRate { get; set; } = Rate.Yearly;
        public AssetType AssetType { get; set; } = AssetType.Undefined;
    }
}
