namespace Database.Entities
{
    public class StockPriceCache
    {
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
