namespace Database.Entities
{
    public enum AssetType
    {
        Undefined = 0,
        Cash = 1,           // Waluta
        Stocks = 2,         // Akcje
        Metals = 3,         // Kruszce/Minerały (złoto, srebro, etc.)
        RealEstate = 4,     // Nieruchomości (na przyszłość)
        Crypto = 5,         // Kryptowaluty (na przyszłość)
        Other = 99,         // Inne
    }
}
